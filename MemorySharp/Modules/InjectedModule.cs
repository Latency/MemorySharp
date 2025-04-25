// ****************************************************************************
// Project:  MemorySharp
// File:     InjectedModule.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

using System.Diagnostics;
using MemorySharp.Internals;
using MemorySharp.Memory;

namespace MemorySharp.Modules;

/// <summary>
///     Class representing an injected module in a remote process.
/// </summary>
public class InjectedModule(ProcessModule module, ModuleFactory moduleFactory, bool mustBeDisposed = true) : RemoteModule(module, moduleFactory), IDisposableState
{
    #region Properties

    #region IsDisposed (implementation of IDisposableState)

    /// <summary>
    ///     Gets a value indicating whether the element is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    #endregion

    #region MustBeDisposed (implementation of IDisposableState)

    /// <summary>
    ///     Gets a value indicating whether the element must be disposed when the Garbage Collector collects the object.
    /// </summary>
    public bool MustBeDisposed => mustBeDisposed;

    #endregion

    #endregion

    #region Constructor/Destructor

    /// <summary>
    ///     Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
    /// </summary>
    ~InjectedModule()
    {
        if (mustBeDisposed)
            Dispose();
    }

    #endregion

    #region Methods

    #region Dispose (implementation of IDisposableState)

    /// <summary>
    ///     Releases all resources used by the <see cref="InjectedModule" /> object.
    /// </summary>
    public virtual void Dispose()
    {
        if (!IsDisposed)
        {
            // Set the flag to true
            IsDisposed = true;
            // Eject the module
            moduleFactory.Eject(this);
            // Avoid the finalizer
            GC.SuppressFinalize(this);
        }
    }

    #endregion Dispose (implementation of IDisposableState)

    #region InternalInject (internal)

    /// <summary>
    ///     Injects the specified module into the address space of the remote process.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp" /> object.</param>
    /// <param name="path">
    ///     The path of the module. This can be either a library module (a .dll file) or an executable module
    ///     (an .exe file).
    /// </param>
    /// <returns>A new instance of the <see cref="InjectedModule" />class.</returns>
    /// <remarks>
    ///     The function GetExitCode is no longer used to get the base address of the injected library, because the returned
    ///     value is
    ///     stored in 32-bit, which is not compatible with 64-bit architecture.
    /// </remarks>
    internal static InjectedModule? InternalInject(MemorySharp memorySharp, string path)
    {
        var def = memorySharp["kernel32"]["LoadLibraryA"];
        if (def is null)
            throw new NullReferenceException("Unable to find LoadLibraryA function.");

        // Call LoadLibraryA remotely
        var thread = memorySharp.Threads.CreateAndJoin((nint)def.Rid, path);
        // Get the inject module
        var module = memorySharp.Modules.NativeModules.First(m => m.FileName == path);
        return thread.GetExitCode<IntPtr>() != IntPtr.Zero
                   ? new InjectedModule(module, memorySharp.Modules)
                   {
                       Region = new RemoteRegion(memorySharp, module.BaseAddress)
                   }
                   : null;
    }

    #endregion InternalInject

    #endregion Methods
}