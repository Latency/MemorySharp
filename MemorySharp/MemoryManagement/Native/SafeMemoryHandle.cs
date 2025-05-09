﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace Binarysharp.MemoryManagement.Native;

/// <summary>
/// Represents a Win32 handle safely managed.
/// </summary>
public sealed class SafeMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    /// <summary>
    /// Parameterless constructor for handles built by the system (like <see cref="NativeMethods.OpenProcess"/>).
    /// </summary>
    public SafeMemoryHandle() : base(true) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeMemoryHandle"/> class, specifying the handle to keep in safe.
    /// </summary>
    /// <param name="handle">The handle to keep in safe.</param>
    public SafeMemoryHandle(nint handle) : base(true) => SetHandle(handle);

    /// <summary>
    /// Executes the code required to free the handle.
    /// </summary>
    /// <returns>True if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.</returns>
    protected override bool ReleaseHandle() =>
        // Check whether the handle is set AND whether the handle has been successfully closed
        handle != nint.Zero && NativeMethods.CloseHandle(handle);

    /// <summary>
    /// Displays the handle value.
    /// </summary>
    /// <returns>Hexadecimal address</returns>
    [DebuggerStepThrough]
    public override string ToString() => $"0x{handle:X}";
}