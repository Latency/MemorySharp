﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using MemorySharp.Internals;
using MemorySharp.Memory;
using System.Diagnostics;

namespace MemorySharp.MemoryManagement.Modules;

/// <summary>
/// Class providing tools for manipulating modules and libraries.
/// </summary>
public class ModuleFactory : IFactory
{
    /// <summary>
    /// The reference of the <see cref="MemoryManagement.MemorySharp"/> object.
    /// </summary>
    protected readonly MemorySharp MemorySharp;

    /// <summary>
    /// The list containing all injected modules (writable).
    /// </summary>
    protected readonly List<InjectedModule> InternalInjectedModules;

    /// <summary>
    /// A collection containing all injected modules.
    /// </summary>
    public IEnumerable<InjectedModule> InjectedModules => InternalInjectedModules.AsReadOnly();

    /// <summary>
    /// Gets the main module for the remote process.
    /// </summary>
    public RemoteModule MainModule { get; private set; }

    /// <summary>
    /// Gets the modules that have been loaded in the remote process.
    /// </summary>
    public IEnumerable<RemoteModule> RemoteModules =>
        // Yield managed modules for ones contained in the target process
        NativeModules.Select(FetchModule);

    /// <summary>
    /// Gets the native modules that have been loaded in the remote process.
    /// </summary>
    internal IEnumerable<ProcessModule> NativeModules => MemorySharp.Native.Modules.Cast<ProcessModule>();

    /// <summary>
    /// Gets a pointer from the remote process.
    /// </summary>
    /// <param name="address">The address of the pointer.</param>
    /// <returns>A new instance of a <see cref="RemotePointer"/> class.</returns>
    public RemotePointer this[nint address] => new(MemorySharp, address);

    /// <summary>
    /// Gets the specified module in the remote process.
    /// </summary>
    /// <param name="moduleName">The name of module (not case sensitive).</param>
    /// <returns>A new instance of a <see cref="RemoteModule"/> class.</returns>
    public RemoteModule this[string moduleName] => FetchModule(moduleName).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleFactory"/> class.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemoryManagement.MemorySharp"/> object.</param>
    internal ModuleFactory(MemorySharp memorySharp)
    {
        // Save the parameter
        MemorySharp = memorySharp;

        // Create a list containing all injected modules
        InternalInjectedModules = [];

        // Save a reference of the main module (the main module is required for a lot of operations, cached for speed reasons)
        MainModule = FetchModule(MemorySharp.Native.MainModule ?? throw new ArgumentNullException());
    }

    /// <summary>
    /// Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
    /// </summary>
    ~ModuleFactory() => Dispose();

    /// <summary>
    /// Releases all resources used by the <see cref="ModuleFactory"/> object.
    /// </summary>
    public virtual void Dispose()
    {
        // Release all injected modules which must be disposed
        foreach (var injectedModule in InternalInjectedModules.Where(m => m.MustBeDisposed))
        {
            injectedModule.Dispose();
        }
        // Clean the cached functions related to this process
        foreach (var cachedFunction in RemoteModule.CachedFunctions.ToArray())
        {
            if (cachedFunction.Key.Item2 == MemorySharp.Handle)
                RemoteModule.CachedFunctions.Remove(cachedFunction);
        }
        // Avoid the finalizer
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
    /// </summary>
    /// <param name="module">The module to eject.</param>
    public void Eject(RemoteModule module)
    {
        // If the module is valid
        if (!module.IsValid) return;

        // Find if the module is an injected one
        var injected = InternalInjectedModules.FirstOrDefault(m => m.Equals(module));
        if (injected != null)
            InternalInjectedModules.Remove(injected);

        // Eject the module
        RemoteModule.InternalEject(MemorySharp, module);
    }

    /// <summary>
    /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
    /// </summary>
    /// <param name="moduleName">The name of module to eject.</param>
    public void Eject(string moduleName)
    {
        // Fint the module to eject
        var module = RemoteModules.FirstOrDefault(m => m.Name == moduleName);
        // Eject the module is it's valid
        if(module != null)
            RemoteModule.InternalEject(MemorySharp, module);
    }

    /// <summary>
    /// Fetches a module from the remote process.
    /// </summary>
    /// <param name="moduleName">A module name (not case sensitive). If the file name extension is omitted, the default library extension .dll is appended.</param>
    /// <returns>A new instance of a <see cref="RemoteModule"/> class.</returns>
    protected async Task<RemoteModule> FetchModule(string moduleName)
    {
        // Convert module name with lower chars
        //moduleName = moduleName.ToLower();

        // Check if the module name has an extension
        if (!Path.HasExtension(moduleName))
            moduleName += ".dll";

        ProcessModule? mod;

        do
        {
            var modules = NativeModules.OrderBy(m => m.FileName).ToList();
            mod = modules.FirstOrDefault(Condition);
            if (mod is not null)
                break;

            await Task.Delay(TimeSpan.FromMilliseconds(500)).ConfigureAwait(false);
            MemorySharp.Native.Refresh();
        }
        while (mod == null);

        if (mod is null)
            throw new NullReferenceException();

        // Fetch and return the module
        return new RemoteModule(MemorySharp, mod);

        bool Condition(ProcessModule m) => m.ModuleName.Equals(moduleName, StringComparison.CurrentCultureIgnoreCase);
    }

    /// <summary>
    /// Fetches a module from the remote process.
    /// </summary>
    /// <param name="module">A module in the remote process.</param>
    /// <returns>A new instance of a <see cref="RemoteModule"/> class.</returns>
    private RemoteModule FetchModule(ProcessModule module) => FetchModule(module.ModuleName).ConfigureAwait(false).GetAwaiter().GetResult();

    /// <summary>
    /// Injects the specified module into the address space of the remote process.
    /// </summary>
    /// <param name="path">The path of the module. This can be either a library module (a .dll file) or an executable module (an .exe file).</param>
    /// <param name="mustBeDisposed">The module will be ejected when the finalizer collects the object.</param>
    /// <returns>A new instance of the <see cref="InjectedModule"/>class.</returns>
    public InjectedModule? Inject(string path, bool mustBeDisposed = true)
    {
        // Injects the module
        var module = InjectedModule.InternalInject(MemorySharp, path);
        if (module is not null)
            InternalInjectedModules.Add(module); // Add the module in the list

        // Return the module
        return module;
    }
}