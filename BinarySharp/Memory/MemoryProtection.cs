﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Native;

namespace Binarysharp.Memory;

/// <summary>
/// Class providing tools for manipulating memory protection.
/// </summary>
public class MemoryProtection : IDisposable
{
    /// <summary>
    /// The reference of the <see cref="MemorySharp"/> object.
    /// </summary>
    private readonly MemorySharp _memorySharp;

    /// <summary>
    /// The base address of the altered memory.
    /// </summary>
    public nint BaseAddress { get; private set; }

    /// <summary>
    /// States if the <see cref="MemoryProtection"/> object nust be disposed when it is collected.
    /// </summary>
    public bool MustBeDisposed { get; set; }

    /// <summary>
    /// Defines the new protection applied to the memory.
    /// </summary>
    public MemoryProtectionFlags NewProtection { get; private set; }

    /// <summary>
    /// References the inital protection of the memory.
    /// </summary>
    public MemoryProtectionFlags OldProtection { get; private set; }

    /// <summary>
    /// The size of the altered memory.
    /// </summary>
    public int Size { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryProtection"/> class.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp"/> object.</param>
    /// <param name="baseAddress">The base address of the memory to change the protection.</param>
    /// <param name="size">The size of the memory to change.</param>
    /// <param name="protection">The new protection to apply.</param>
    /// <param name="mustBeDisposed">The resource will be automatically disposed when the finalizer collects the object.</param>
    public MemoryProtection(MemorySharp memorySharp, nint baseAddress, int size, MemoryProtectionFlags protection = MemoryProtectionFlags.ExecuteReadWrite, bool mustBeDisposed = true)
    {
        // Save the parameters
        _memorySharp   = memorySharp;
        BaseAddress    = baseAddress;
        NewProtection  = protection;
        Size           = size;
        MustBeDisposed = mustBeDisposed;

        // Change the memory protection
        OldProtection = MemoryCore.ChangeProtection(_memorySharp.Handle, baseAddress, size, protection);
    }

    /// <summary>
    /// Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
    /// </summary>
    ~MemoryProtection()
    {
        if (MustBeDisposed)
            Dispose();
    }

    /// <summary>
    /// Restores the initial protection of the memory.
    /// </summary>
    public virtual void Dispose()
    {
        // Restore the memory protection
        MemoryCore.ChangeProtection(_memorySharp.Handle, BaseAddress, Size, OldProtection);

        // Avoid the finalizer 
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString() => $"BaseAddress = 0x{BaseAddress.ToInt64():X} NewProtection = {NewProtection} OldProtection = {OldProtection}";
}