/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using MemorySharp.Helpers;
using MemorySharp.Internals;
using MemorySharp.Memory;
using MemorySharp.MemoryManagement.Modules;
using MemorySharp.MemoryManagement.Native;
using MemorySharp.Threading;
using MemorySharp.Windows;

namespace MemorySharp.MemoryManagement;

/// <summary>
/// Class for memory editing a remote process.
/// </summary>
public class MemorySharp : IDisposable, IEquatable<MemorySharp>
{
    /// <summary>
    /// Raises when the <see cref="MemorySharp"/> object is disposed.
    /// </summary>
    public event EventHandler? OnDispose;

    /// <summary>
    /// The factories embedded inside the library.
    /// </summary>
    protected List<IFactory> Factories;

    /// <summary>
    /// State if the process is running.
    /// </summary>
    public bool IsRunning => Handle is { IsInvalid: false, IsClosed: false } && !Native.HasExited;

    /// <summary>
    /// The remote process handle opened with all rights.
    /// </summary>
    public SafeMemoryHandle Handle { get; }

    /// <summary>
    /// Factory for manipulating memory space.
    /// </summary>
    public MemoryFactory Memory { get; protected set; }

    /// <summary>
    /// Factory for manipulating modules and libraries.
    /// </summary>
    public ModuleFactory Modules { get; protected set; }

    /// <summary>
    /// Provide access to the opened process.
    /// </summary>
    public Process Native { get; }

    /// <summary>
    /// The Process Environment Block of the process.
    /// </summary>
    public ManagedPeb Peb { get; }

    /// <summary>
    /// Gets the unique identifier for the remote process.
    /// </summary>
    public int Pid => Native.Id;

    /// <summary>
    /// Gets the specified module in the remote process.
    /// </summary>
    /// <param name="moduleName">The name of module (not case sensitive).</param>
    /// <returns>A new instance of a <see cref="RemoteModule"/> class.</returns>
    public RemoteModule this[string moduleName] => Modules[moduleName];

    /// <summary>
    /// Gets a pointer to the specified address in the remote process.
    /// </summary>
    /// <param name="address">The address pointed.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    /// <returns>A new instance of a <see cref="RemotePointer"/> class.</returns>
    public RemotePointer this[nint address, bool isRelative = true] => new(this, isRelative ? MakeAbsolute(address) : address);

    /// <summary>
    /// Factory for manipulating threads.
    /// </summary>
    public ThreadFactory Threads { get; protected set; }

    /// <summary>
    /// Factory for manipulating windows.
    /// </summary>
    public WindowFactory Windows { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemorySharp"/> class.
    /// </summary>
    /// <param name="process">Process to open.</param>
    public MemorySharp(Process process)
    {
        // Save the reference of the process
        Native = process;

        // Open the process with all rights
        Handle = MemoryCore.OpenProcess(ProcessAccessFlags.AllAccess, process.Id);

        // Initialize the PEB
        //Peb = new ManagedPeb(this, ManagedPeb.FindPeb(Handle));

        // Create instances of the factories
        Factories = [];
        Factories.AddRange([
            Memory   = new MemoryFactory(this),
            Modules  = new ModuleFactory(this),
            Threads  = new ThreadFactory(this),
            Windows  = new WindowFactory(this)
        ]);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemorySharp"/> class.
    /// </summary>
    /// <param name="processId">Process id of the process to open.</param>
    public MemorySharp(int processId)
        : this(ApplicationFinder.FromProcessId(processId))
    { }

    /// <summary>
    /// Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
    /// </summary>
    ~MemorySharp() => Dispose();

    /// <summary>
    /// Releases all resources used by the <see cref="MemorySharp"/> object.
    /// </summary>
    public virtual void Dispose()
    {
        // Raise the event OnDispose
        OnDispose?.Invoke(this, EventArgs.Empty);

        // Dispose all factories
        Factories.ForEach(factory => factory.Dispose());

        // Close the process handle
        Handle.Close();

        // Avoid the finalizer
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((MemorySharp)obj);
    }

    /// <summary>
    /// Suspends the process.
    /// </summary>
    public void Suspend() => Native.SuspendProcess();

    /// <summary>
    /// Resumes the process.
    /// </summary>
    public void Resume() => Native.ResumeProcess();

    /// <summary>
    /// Returns a value indicating whether this instance is equal to a specified object.
    /// </summary>
    public bool Equals(MemorySharp? other) => !ReferenceEquals(null, other) && (ReferenceEquals(this, other) || Handle.Equals(other.Handle));

    /// <summary>
    /// Serves as a hash function for a particular type.
    /// </summary>
    public override int GetHashCode() => Handle.GetHashCode();

    /// <summary>
    /// Makes an absolute address from a relative one based on the main module.
    /// </summary>
    /// <param name="address">The relative address.</param>
    /// <returns>The absolute address.</returns>
    public nint MakeAbsolute(nint address)
    {
        // Check if the relative address is not greater than the main module size
        if (address.ToInt64() > Modules.MainModule.Size)
            throw new ArgumentOutOfRangeException(nameof(address), "The relative address cannot be greater than the main module size.");
        // Compute the absolute address
        return new nint(Modules.MainModule.BaseAddress.ToInt64() + address.ToInt64());
    }

    /// <summary>
    /// Makes a relative address from an absolute one based on the main module.
    /// </summary>
    /// <param name="address">The absolute address.</param>
    /// <returns>The relative address.</returns>
    public nint MakeRelative(nint address)
    {
        // Check if the absolute address is smaller than the main module base address
        if (address.ToInt64() < Modules.MainModule.BaseAddress.ToInt64())
            throw new ArgumentOutOfRangeException(nameof(address), "The absolute address cannot be smaller than the main module base address.");
        // Compute the relative address
        return new nint(address.ToInt64() - Modules.MainModule.BaseAddress.ToInt64());
    }

    public static bool operator ==(MemorySharp left, MemorySharp right) => Equals(left, right);

    public static bool operator !=(MemorySharp? left, MemorySharp? right) => !Equals(left, right);

    /// <summary>
    /// Reads the value of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="address">The address where the value is read.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    /// <returns>A value.</returns>
    public T? Read<T>(nint address, bool isRelative = true) => MarshalType<T>.ByteArrayToObject(ReadBytes(address, MarshalType<T>.Size, isRelative));

    /// <summary>
    /// Reads the value of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="address">The address where the value is read.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    /// <returns>A value.</returns>
    public T? Read<T>(Enum address, bool isRelative = true) => Read<T>(new nint(Convert.ToInt64(address)), isRelative);

    /// <summary>
    /// Reads an array of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="address">The address where the values is read.</param>
    /// <param name="count">The number of cells in the array.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    /// <returns>An array.</returns>
    public T?[] Read<T>(nint address, int count, bool isRelative = true)
    {
        // Allocate an array to store the results
        var array = new T?[count];

        // Read all the memory at once, much faster then reading each time individually
        var bytes = ReadBytes(address, MarshalType<T>.Size * count, isRelative);

        // If we check the type we can gain an additional boost of speed
        if (typeof (T) != typeof (byte))
            for (var i = 0; i < count; i++)
                array[i] = MarshalType<T>.ByteArrayToObject(bytes, MarshalType<T>.Size * i);
        else // Just copy the bytes
            Buffer.BlockCopy(bytes, 0, array, 0, count);

        return array;
    }

    /// <summary>
    /// Reads an array of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="address">The address where the values is read.</param>
    /// <param name="count">The number of cells in the array.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    /// <returns>An array.</returns>
    public T?[] Read<T>(Enum address, int count, bool isRelative = true) => Read<T>(new nint(Convert.ToInt64(address)), count, isRelative);

    /// <summary>
    /// Reads an array of bytes in the remote process.
    /// </summary>
    /// <param name="address">The address where the array is read.</param>
    /// <param name="count">The number of cells.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    /// <returns>The array of bytes.</returns>
    protected byte[] ReadBytes(nint address, int count, bool isRelative = true) => MemoryCore.ReadBytes(Handle, isRelative ? MakeAbsolute(address) : address, count);

    /// <summary>
    /// Reads a string with a specified encoding in the remote process.
    /// </summary>
    /// <param name="address">The address where the string is read.</param>
    /// <param name="encoding">The encoding used.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    /// <param name="maxLength">[Optional] The number of maximum bytes to read. The string is automatically cropped at this end ('\0' char).</param>
    /// <returns>The string.</returns>
    public string ReadString(nint address, Encoding encoding, bool isRelative = true, int maxLength = 512)
    {
        // Read the string
        var data = encoding.GetString(ReadBytes(address, maxLength, isRelative));

        // Search the end of the string
        var endOfStringPosition = data.IndexOf('\0');

        // Crop the string with this end if found, return the string otherwise
        return endOfStringPosition == -1 ? data : data[..endOfStringPosition];
    }

    /// <summary>
    /// Reads a string with a specified encoding in the remote process.
    /// </summary>
    /// <param name="address">The address where the string is read.</param>
    /// <param name="encoding">The encoding used.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    /// <param name="maxLength">[Optional] The number of maximum bytes to read. The string is automatically cropped at this end ('\0' char).</param>
    /// <returns>The string.</returns>
    public string ReadString(Enum address, Encoding encoding, bool isRelative = true, int maxLength = 512) => ReadString(new nint(Convert.ToInt64(address)), encoding, isRelative, maxLength);

    /// <summary>
    /// Reads a string using the encoding UTF8 in the remote process.
    /// </summary>
    /// <param name="address">The address where the string is read.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    /// <param name="maxLength">[Optional] The number of maximum bytes to read. The string is automatically cropped at this end ('\0' char).</param>
    /// <returns>The string.</returns>
    public string ReadString(nint address, bool isRelative = true, int maxLength = 512) => ReadString(address, Encoding.UTF8, isRelative, maxLength);

    /// <summary>
    /// Reads a string using the encoding UTF8 in the remote process.
    /// </summary>
    /// <param name="address">The address where the string is read.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    /// <param name="maxLength">[Optional] The number of maximum bytes to read. The string is automatically cropped at this end ('\0' char).</param>
    /// <returns>The string.</returns>
    public string ReadString(Enum address, bool isRelative = true, int maxLength = 512) => ReadString(new nint(Convert.ToInt64(address)), isRelative, maxLength);

    /// <summary>
    /// Writes the values of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="address">The address where the value is written.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    public void Write<T>(nint address, [DisallowNull] T value, bool isRelative = true) => WriteBytes(address, MarshalType<T>.ObjectToByteArray(value), isRelative);

    /// <summary>
    /// Writes the values of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="address">The address where the value is written.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    public void Write<T>(Enum address, [DisallowNull] T value, bool isRelative = true) => Write(new nint(Convert.ToInt64(address)), value, isRelative);

    /// <summary>
    /// Writes an array of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="address">The address where the values is written.</param>
    /// <param name="array">The array to write.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    /// <returns>Number of bytes written.</returns>
    public uint Write<T>(nint address, T[] array, bool isRelative = true)
    {
        // Allocate an array containing the values of the array converted into bytes
        var valuesInBytes = new byte[MarshalType<T>.Size * array.Length];

        // Convert each value into its bytes representation
        for (var i = 0; i < array.Length; i++)
        {
            var offsetInArray = MarshalType<T>.Size * i;
            var elmt          = array[i];
            if (elmt is null)
                throw new ArgumentNullException();

            Buffer.BlockCopy(MarshalType<T>.ObjectToByteArray(elmt), 0, valuesInBytes, offsetInArray, MarshalType<T>.Size);
        }

        return WriteBytes(address, valuesInBytes, isRelative);
    }

    /// <summary>
    /// Writes an array of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="address">The address where the values is written.</param>
    /// <param name="array">The array to write.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    public void Write<T>(Enum address, T[] array, bool isRelative = true) => Write(new nint(Convert.ToInt64(address)), array, isRelative);

    /// <summary>
    /// Write an array of bytes in the remote process.
    /// </summary>
    /// <param name="address">The address where the array is written.</param>
    /// <param name="byteArray">The array of bytes to write.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    /// <returns>Number of bytes written</returns>
    protected uint WriteBytes(nint address, byte[] byteArray, bool isRelative = true)
    {
        // Change the protection of the memory to allow writable
        using (new MemoryProtection(this, isRelative ? MakeAbsolute(address) : address, MarshalType<byte>.Size * byteArray.Length))
            return MemoryCore.WriteBytes(Handle, isRelative ? MakeAbsolute(address) : address, byteArray);
    }

    /// <summary>
    /// Writes a string with a specified encoding in the remote process.
    /// </summary>
    /// <param name="address">The address where the string is written.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="encoding">The encoding used.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    public void WriteString(nint address, string text, Encoding encoding, bool isRelative = true) => WriteBytes(address, encoding.GetBytes($"{text}\0"), isRelative);

    /// <summary>
    /// Writes a string with a specified encoding in the remote process.
    /// </summary>
    /// <param name="address">The address where the string is written.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="encoding">The encoding used.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    public void WriteString(Enum address, string text, Encoding encoding, bool isRelative = true) => WriteString(new nint(Convert.ToInt64(address)), text, encoding, isRelative);

    /// <summary>
    /// Writes a string using the encoding UTF8 in the remote process.
    /// </summary>
    /// <param name="address">The address where the string is written.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    public void WriteString(nint address, string text, bool isRelative = true) => WriteString(address, text, Encoding.UTF8, isRelative);

    /// <summary>
    /// Writes a string using the encoding UTF8 in the remote process.
    /// </summary>
    /// <param name="address">The address where the string is written.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="isRelative">[Optional] State if the address is relative to the main module.</param>
    public void WriteString(Enum address, string text, bool isRelative = true) => WriteString(new nint(Convert.ToInt64(address)), text, isRelative);
}