﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System.ComponentModel;
using System.Diagnostics;
using MemorySharp.MemoryManagement.Native;

namespace MemorySharp.MemoryManagement.Modules;

/// <summary>
/// Static core class providing tools for manipulating modules and libraries.
/// </summary>
public static class ModuleCore
{
    /// <summary>
    /// Retrieves the address of an exported function or variable from the specified dynamic-link library (DLL).
    /// </summary>
    /// <param name="module">The module.</param>
    /// <param name="functionName">The function or variable name, or the function's ordinal value.</param>
    /// <returns>The address of the exported function.</returns>
    public static nint GetProcAddress(ProcessModule module, string functionName)
    {
        // Get the function address
        var ret = NativeMethods.GetProcAddress(module.BaseAddress, functionName);

        // Check whether the function was found
        if (ret != nint.Zero)
            return ret;

        // Else the function was not found, throws an exception
        throw new Win32Exception($"Couldn't get the function address of {functionName}.");
    }

    /// <summary>
    /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
    /// </summary>
    /// <param name="libraryName">The name of the library to free (not case-sensitive).</param>
    public static void FreeLibrary(string libraryName)
    {
        // Get the module
        var module = Process.GetCurrentProcess().Modules.Cast<ProcessModule>().FirstOrDefault(m => string.Equals(m.ModuleName, libraryName, StringComparison.CurrentCultureIgnoreCase));

        // Check whether there is a library loaded with this name
        if(module == null)
            throw new ArgumentException($"Couldn't free the library {libraryName} because it doesn't exist in the current process.");

        // Free the library
        if(!NativeMethods.FreeLibrary(module.BaseAddress))
            throw new Win32Exception($"Couldn't free the library {libraryName}.");
    }

    /// <summary>
    /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
    /// </summary>
    /// <param name="module">The <see cref="ProcessModule"/> object corresponding to the library to free.</param>
    public static void FreeLibrary(ProcessModule module) => FreeLibrary(module.ModuleName);

    /// <summary>
    /// Loads the specified module into the address space of the calling process.
    /// </summary>
    /// <param name="libraryPath">The name of the module. This can be either a library module (a .dll file) or an executable module (an .exe file).</param>
    /// <returns>A <see cref="ProcessModule"/> corresponding to the loaded library.</returns>
    public static ProcessModule LoadLibrary(string libraryPath)
    {
        // Check whether the file exists
        if(!File.Exists(libraryPath))
            throw new FileNotFoundException($"Couldn't load the library {libraryPath} because the file doesn't exist.");

        // Load the library
        if(NativeMethods.LoadLibrary(libraryPath) == nint.Zero)
            throw new Win32Exception($"Couldn't load the library {libraryPath}.");

        // Enumerate the loaded modules and return the one newly added
        return Process.GetCurrentProcess().Modules.Cast<ProcessModule>().First(m => m.FileName == libraryPath);
    }
}