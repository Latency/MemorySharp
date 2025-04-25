// ****************************************************************************
// Project:  MemorySharp
// File:     RemotePointer.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************
// ReSharper disable UnusedMember.Global

using MemorySharp.Assembly.CallingConvention;
using MemorySharp.Native;
using System.Text;

namespace MemorySharp.Memory;

/// <summary>
///     Class representing a pointer in the memory of the remote process.
/// </summary>
public class RemotePointer(MemorySharp memorySharp, IntPtr baseAddress) : IEquatable<RemotePointer>
{
    #region Properties

    #region BaseAddress

    /// <summary>
    ///     The address of the pointer in the remote process.
    /// </summary>
    public IntPtr BaseAddress { get; protected set; } = baseAddress;

    #endregion BaseAddress

    #region MemorySharp

    /// <summary>
    ///     The reference of the <see cref="MemorySharp" /> object.
    /// </summary>
    internal MemorySharp MemorySharp => memorySharp;

    #endregion MemorySharp

    #region IsValid

    /// <summary>
    ///     Gets if the <see cref="RemotePointer" /> is valid.
    /// </summary>
    public virtual bool IsValid => memorySharp.IsRunning && BaseAddress != IntPtr.Zero;

    #endregion IsValid

    #endregion Properties

    #region Methods

    #region ChangeProtection

    /// <summary>
    ///     Changes the protection of the n next bytes in remote process.
    /// </summary>
    /// <param name="size">The size of the memory to change.</param>
    /// <param name="protection">The new protection to apply.</param>
    /// <param name="mustBeDisposed">The resource will be automatically disposed when the finalizer collects the object.</param>
    /// <returns>A new instance of the <see cref="MemoryProtection" /> class.</returns>
    public MemoryProtection ChangeProtection(IntPtr size, MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite, bool mustBeDisposed = true) => new(memorySharp, BaseAddress, size, protection, mustBeDisposed);

    #endregion ChangeProtection

    #region Equals (override)

    /// <summary>
    ///     Determines whether the specified object is equal to the current object.
    /// </summary>
    public override bool Equals(object? obj) => !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((RemotePointer)obj));

    /// <summary>
    ///     Returns a value indicating whether this instance is equal to a specified object.
    /// </summary>
    public bool Equals(RemotePointer? other) => other is not null && (ReferenceEquals(this, other) || (BaseAddress.Equals(other.BaseAddress) && memorySharp.Equals(other.MemorySharp)));

    #endregion Equals (override)

    #region Execute

    /// <summary>
    ///     Executes the assembly code in the remote process.
    /// </summary>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T Execute<T>() => memorySharp.Assembly.Execute<T>(BaseAddress);

    /// <summary>
    ///     Executes the assembly code in the remote process.
    /// </summary>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public IntPtr Execute() => Execute<IntPtr>();

    /// <summary>
    ///     Executes the assembly code in the remote process.
    /// </summary>
    /// <param name="parameter">The parameter used to execute the assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T Execute<T>(dynamic parameter) => memorySharp.Assembly.Execute<T>(BaseAddress, parameter);

    /// <summary>
    ///     Executes the assembly code in the remote process.
    /// </summary>
    /// <param name="parameter">The parameter used to execute the assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public IntPtr Execute(dynamic parameter) => Execute<IntPtr>(parameter);

    /// <summary>
    ///     Executes the assembly code in the remote process.
    /// </summary>
    /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
    /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public T Execute<T>(CallingConventions callingConvention, params dynamic[] parameters) => memorySharp.Assembly.Execute<T>(BaseAddress, callingConvention, parameters);

    /// <summary>
    ///     Executes the assembly code in the remote process.
    /// </summary>
    /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
    /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
    /// <returns>The return value is the exit code of the thread created to execute the assembly code.</returns>
    public IntPtr Execute(CallingConventions callingConvention, params dynamic[] parameters) => Execute<IntPtr>(callingConvention, parameters);

    #endregion Execute

    #region ExecuteAsync

    /// <summary>
    ///     Executes asynchronously the assembly code in the remote process.
    /// </summary>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<T> ExecuteAsync<T>() => memorySharp.Assembly.ExecuteAsync<T>(BaseAddress);

    /// <summary>
    ///     Executes asynchronously the assembly code in the remote process.
    /// </summary>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<IntPtr> ExecuteAsync() => ExecuteAsync<IntPtr>();

    /// <summary>
    ///     Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="parameter">The parameter used to execute the assembly code.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<T> ExecuteAsync<T>(dynamic parameter) => memorySharp.Assembly.ExecuteAsync<T>(BaseAddress, parameter);

    /// <summary>
    ///     Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="parameter">The parameter used to execute the assembly code.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<IntPtr> ExecuteAsync(dynamic parameter) => ExecuteAsync<IntPtr>(parameter);

    /// <summary>
    ///     Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
    /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<T> ExecuteAsync<T>(CallingConventions callingConvention, params dynamic[] parameters) => memorySharp.Assembly.ExecuteAsync<T>(BaseAddress, callingConvention, parameters);

    /// <summary>
    ///     Executes asynchronously the assembly code located in the remote process at the specified address.
    /// </summary>
    /// <param name="callingConvention">The calling convention used to execute the assembly code with the parameters.</param>
    /// <param name="parameters">An array of parameters used to execute the assembly code.</param>
    /// <returns>
    ///     The return value is an asynchronous operation that return the exit code of the thread created to execute the
    ///     assembly code.
    /// </returns>
    public Task<IntPtr> ExecuteAsync(CallingConventions callingConvention, params dynamic[] parameters) => ExecuteAsync<IntPtr>(callingConvention, parameters);

    #endregion ExecuteAsync

    #region GetHashCode (override)

    /// <summary>
    ///     Serves as a hash function for a particular type.
    /// </summary>
    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => BaseAddress.GetHashCode() ^ memorySharp.GetHashCode();

    #endregion GetHashCode (override)

    #region Operator (override)

    public static bool operator ==(RemotePointer left, RemotePointer right) => Equals(left, right);

    public static bool operator !=(RemotePointer left, RemotePointer right) => !Equals(left, right);

    #endregion Operator (override)

    #region Read

    /// <summary>
    ///     Reads the value of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="offset">The offset where the value is read from the pointer.</param>
    /// <returns>A value.</returns>
    public T Read<T>(int offset) => memorySharp.Read<T>(BaseAddress + offset, false);

    /// <summary>
    ///     Reads the value of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="offset">The offset where the value is read from the pointer.</param>
    /// <returns>A value.</returns>
    public T Read<T>(Enum offset) => Read<T>(Convert.ToInt32(offset));

    /// <summary>
    ///     Reads the value of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <returns>A value.</returns>
    public T Read<T>() => Read<T>(0);

    /// <summary>
    ///     Reads an array of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="offset">The offset where the values is read from the pointer.</param>
    /// <param name="count">The number of cells in the array.</param>
    /// <returns>An array.</returns>
    public T[] Read<T>(int offset, int count) => memorySharp.Read<T>(BaseAddress + offset, count, false);

    /// <summary>
    ///     Reads an array of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="offset">The offset where the values is read from the pointer.</param>
    /// <param name="count">The number of cells in the array.</param>
    /// <returns>An array.</returns>
    public T[] Read<T>(Enum offset, int count) => Read<T>(Convert.ToInt32(offset), count);

    #endregion Read

    #region ReadString

    /// <summary>
    ///     Reads a string with a specified encoding in the remote process.
    /// </summary>
    /// <param name="offset">The offset where the string is read from the pointer.</param>
    /// <param name="encoding">The encoding used.</param>
    /// <param name="maxLength">
    ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
    ///     ('\0' char).
    /// </param>
    /// <returns>The string.</returns>
    public string ReadString(int offset, Encoding encoding, int maxLength = 512) => memorySharp.ReadString(BaseAddress + offset, encoding, false, maxLength);

    /// <summary>
    ///     Reads a string with a specified encoding in the remote process.
    /// </summary>
    /// <param name="offset">The offset where the string is read from the pointer.</param>
    /// <param name="encoding">The encoding used.</param>
    /// <param name="maxLength">
    ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
    ///     ('\0' char).
    /// </param>
    /// <returns>The string.</returns>
    public string ReadString(Enum offset, Encoding encoding, int maxLength = 512) => ReadString(Convert.ToInt32(offset), encoding, maxLength);

    /// <summary>
    ///     Reads a string with a specified encoding in the remote process.
    /// </summary>
    /// <param name="encoding">The encoding used.</param>
    /// <param name="maxLength">
    ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
    ///     ('\0' char).
    /// </param>
    /// <returns>The string.</returns>
    public string ReadString(Encoding encoding, int maxLength = 512) => ReadString(0, encoding, maxLength);

    /// <summary>
    ///     Reads a string using the encoding UTF8 in the remote process.
    /// </summary>
    /// <param name="offset">The offset where the string is read from the pointer.</param>
    /// <param name="maxLength">
    ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
    ///     ('\0' char).
    /// </param>
    /// <returns>The string.</returns>
    public string ReadString(int offset, int maxLength = 512) => memorySharp.ReadString(BaseAddress + offset, false, maxLength);

    /// <summary>
    ///     Reads a string using the encoding UTF8 in the remote process.
    /// </summary>
    /// <param name="offset">The offset where the string is read from the pointer.</param>
    /// <param name="maxLength">
    ///     [Optional] The number of maximum bytes to read. The string is automatically cropped at this end
    ///     ('\0' char).
    /// </param>
    /// <returns>The string.</returns>
    public string ReadString(Enum offset, int maxLength = 512) => ReadString(Convert.ToInt32(offset), maxLength);

    #endregion ReadString

    #region ToString (override)

    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    public override string ToString() => $"BaseAddress = 0x{BaseAddress.ToInt64():X}";

    #endregion ToString (override)

    #region Write

    /// <summary>
    ///     Writes the values of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="offset">The offset where the value is written from the pointer.</param>
    /// <param name="value">The value to write.</param>
    public void Write<T>(int offset, T value) => memorySharp.Write(BaseAddress + offset, value, false);

    /// <summary>
    ///     Writes the values of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="offset">The offset where the value is written from the pointer.</param>
    /// <param name="value">The value to write.</param>
    public void Write<T>(Enum offset, T value) => Write(Convert.ToInt32(offset), value);

    /// <summary>
    ///     Writes the values of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to write.</param>
    public void Write<T>(T value) => Write(0, value);

    /// <summary>
    ///     Writes an array of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="offset">The offset where the values is written from the pointer.</param>
    /// <param name="array">The array to write.</param>
    public void Write<T>(int offset, T[] array) => memorySharp.Write(BaseAddress + offset, array, false);

    /// <summary>
    ///     Writes an array of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="offset">The offset where the values is written from the pointer.</param>
    /// <param name="array">The array to write.</param>
    public void Write<T>(Enum offset, T[] array) => Write(Convert.ToInt32(offset), array);

    /// <summary>
    ///     Writes an array of a specified type in the remote process.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="array">The array to write.</param>
    public void Write<T>(T[] array) => Write(0, array);

    #endregion Write

    #region WriteString

    /// <summary>
    ///     Writes a string with a specified encoding in the remote process.
    /// </summary>
    /// <param name="offset">The offset where the string is written from the pointer.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="encoding">The encoding used.</param>
    public void WriteString(int offset, string text, Encoding encoding) => memorySharp.WriteString(BaseAddress + offset, text, encoding, false);

    /// <summary>
    ///     Writes a string with a specified encoding in the remote process.
    /// </summary>
    /// <param name="offset">The offset where the string is written from the pointer.</param>
    /// <param name="text">The text to write.</param>
    /// <param name="encoding">The encoding used.</param>
    public void WriteString(Enum offset, string text, Encoding encoding) => WriteString(Convert.ToInt32(offset), text, encoding);

    /// <summary>
    ///     Writes a string with a specified encoding in the remote process.
    /// </summary>
    /// <param name="text">The text to write.</param>
    /// <param name="encoding">The encoding used.</param>
    public void WriteString(string text, Encoding encoding) => WriteString(0, text, encoding);

    /// <summary>
    ///     Writes a string using the encoding UTF8 in the remote process.
    /// </summary>
    /// <param name="offset">The offset where the string is written from the pointer.</param>
    /// <param name="text">The text to write.</param>
    public void WriteString(int offset, string text) => memorySharp.WriteString(BaseAddress + offset, text, false);

    /// <summary>
    ///     Writes a string using the encoding UTF8 in the remote process.
    /// </summary>
    /// <param name="offset">The offset where the string is written from the pointer.</param>
    /// <param name="text">The text to write.</param>
    public void WriteString(Enum offset, string text) => WriteString(Convert.ToInt32(offset), text);

    /// <summary>
    ///     Writes a string using the encoding UTF8 in the remote process.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public void WriteString(string text) => WriteString(0, text);

    #endregion WriteString

    #endregion Methods
}