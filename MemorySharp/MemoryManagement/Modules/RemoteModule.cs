﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System.Diagnostics;
using MemorySharp.Memory;
using MemorySharp.MemoryManagement.Native;

namespace MemorySharp.MemoryManagement.Modules;

/// <summary>
/// Class repesenting a module in the remote process.
/// </summary>
public class RemoteModule : RemoteRegion
{
    /// <summary>
    /// The dictionary containing all cached functions of the remote module.
    /// </summary>
    internal static readonly IDictionary<Tuple<string, SafeMemoryHandle>, RemoteFunction> CachedFunctions = new Dictionary<Tuple<string, SafeMemoryHandle>, RemoteFunction>();

    /// <summary>
    /// State if this is the main module of the remote process.
    /// </summary>
    public bool IsMainModule => MemorySharp?.Native.MainModule?.BaseAddress == BaseAddress;

    /// <summary>
    /// Gets if the <see cref="RemoteModule"/> is valid.
    /// </summary>
    public override bool IsValid => base.IsValid && MemorySharp.Native.Modules.Cast<ProcessModule>().Any(m => m.BaseAddress == BaseAddress && m.ModuleName == Name);

    /// <summary>
    /// The name of the module.
    /// </summary>
    public string Name => Native.ModuleName;

    /// <summary>
    /// The native <see cref="ProcessModule"/> object corresponding to this module.
    /// </summary>
    public ProcessModule Native { get; }

    /// <summary>
    /// The full path of the module.
    /// </summary>
    public string Path => Native.FileName;

    /// <summary>
    /// The size of the module in the memory of the remote process.
    /// </summary>
    public int Size => Native.ModuleMemorySize;

    /// <summary>
    /// Gets the specified function in the remote module.
    /// </summary>
    /// <param name="functionName">The name of the function.</param>
    /// <returns>A new instance of a <see cref="RemoteFunction"/> class.</returns>
    public RemoteFunction this[string functionName] => FindFunction(functionName);

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoteModule"/> class.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp"/> object.</param>
    /// <param name="module">The native <see cref="ProcessModule"/> object corresponding to this module.</param>
    internal RemoteModule(MemorySharp memorySharp, ProcessModule module) : base(memorySharp, module.BaseAddress) => Native = module;

    /// <summary>
    /// Ejects the loaded dynamic-link library (DLL) module.
    /// </summary>
    public void Eject()
    {
        // Eject the module
        MemorySharp.Modules.Eject(this);
        // Remove the pointer
        BaseAddress = nint.Zero;
    }

    /// <summary>
    /// Finds the specified function in the remote module.
    /// </summary>
    /// <param name="functionName">The name of the function (case sensitive).</param>
    /// <returns>A new instance of a <see cref="RemoteFunction"/> class.</returns>
    /// <remarks>
    /// Interesting article on how DLL loading works: http://msdn.microsoft.com/en-us/magazine/bb985014.aspx
    /// </remarks>
    public RemoteFunction FindFunction(string functionName)
    {
        // Create the tuple
        var tuple = Tuple.Create(functionName, MemorySharp.Handle);

        // Check if the function is already cached
        if (CachedFunctions.TryGetValue(tuple, out var findFunction))
            return findFunction;

        // Get the offset of the function
        var offset = ModuleCore.GetProcAddress(Native, functionName).ToInt64() - Native.BaseAddress.ToInt64();

        // Rebase the function with the remote module
        var function = new RemoteFunction(MemorySharp, new nint(Native.BaseAddress.ToInt64() + offset), functionName);

        // Store the function in the cache
        CachedFunctions.Add(tuple, function);

        // Return the function rebased with the remote module
        return function;
    }

    /// <summary>
    /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp"/> object.</param>
    /// <param name="module">The module to eject.</param>
    internal static void InternalEject(MemorySharp memorySharp, RemoteModule module) => memorySharp.Threads.CreateAndJoin(memorySharp["kernel32"]["FreeLibrary"].BaseAddress, module.BaseAddress);

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString() => $"BaseAddress = 0x{BaseAddress.ToInt64():X} Name = {Name}";
}