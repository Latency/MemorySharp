/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using Binarysharp.Memory;

namespace Binarysharp.MemoryManagement.Modules;

/// <summary>
/// Class representing a function in the remote process.
/// </summary>
public class RemoteFunction(MemorySharp memorySharp, IntPtr address, string functionName) : RemotePointer(memorySharp, address)
{
    #region Properties
    /// <summary>
    /// The name of the function.
    /// </summary>
    public string Name { get; private set; } = functionName;
    #endregion

    #region Constructor

    // Save the parameter

    #endregion

    #region Methods
    #region ToString (override)
    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString() => $"BaseAddress = 0x{BaseAddress.ToInt64():X} Name = {Name}";

    #endregion
    #endregion
}