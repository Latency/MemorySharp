﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System.Diagnostics;
using Binarysharp.Internals;

namespace Binarysharp.MemoryManagement.Modules;

/// <summary>
/// Class representing an injected module in a remote process.
/// </summary>
public class InjectedModule : RemoteModule, IDisposableState
{
    /// <summary>
    /// Gets a value indicating whether the element is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the element must be disposed when the Garbage Collector collects the object.
    /// </summary>
    public bool MustBeDisposed { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InjectedModule"/> class.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp"/> object.</param>
    /// <param name="module">The native <see cref="ProcessModule"/> object corresponding to the injected module.</param>
    /// <param name="mustBeDisposed">The module will be ejected when the finalizer collects the object.</param>
    internal InjectedModule(MemorySharp memorySharp, ProcessModule module, bool mustBeDisposed = true)
        : base(memorySharp, module)
    {
        // Save the parameter
        MustBeDisposed = mustBeDisposed;
    }

    /// <summary>
    /// Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
    /// </summary>
    ~InjectedModule()
    {
        if(MustBeDisposed)
            Dispose();
    }

    /// <summary>
    /// Releases all resources used by the <see cref="InjectedModule"/> object.
    /// </summary>
    public virtual void Dispose()
    {
        if (!IsDisposed)
        {
            // Set the flag to true
            IsDisposed = true;
            // Eject the module
            MemorySharp.Modules.Eject(this);

            // Avoid the finalizer 
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Injects the specified module into the address space of the remote process.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp"/> object.</param>
    /// <param name="path">The path of the module. This can be either a library module (a .dll file) or an executable module (an .exe file).</param>
    /// <returns>A new instance of the <see cref="InjectedModule"/>class.</returns>
    internal static InjectedModule? InternalInject(MemorySharp memorySharp, string path)
    {
        // Call LoadLibraryA remotely
        var thread = memorySharp.Threads.CreateAndJoin(memorySharp["kernel32"]["LoadLibraryA"].BaseAddress, path);
        // Get the inject module
        return thread.GetExitCode<nint>() != nint.Zero ? new InjectedModule(memorySharp, memorySharp.Modules.NativeModules.First(m => m.BaseAddress == thread.GetExitCode<nint>())) : null;
    }
}