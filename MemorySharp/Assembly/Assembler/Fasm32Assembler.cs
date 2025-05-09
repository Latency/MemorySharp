/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

namespace Binarysharp.Assembly.Assembler;

/// <summary>
/// Implement Fasm.NET compiler for 32-bit development.
/// More info: https://github.com/ZenLulz/Fasm.NET
/// </summary>
public class Fasm32Assembler : IAssembler
{
    /// <summary>
    /// Assemble the specified assembly code.
    /// </summary>
    /// <param name="asm">The assembly code.</param>
    /// <returns>An array of bytes containing the assembly code.</returns>
    public byte[] Assemble(string asm) => Assemble(asm, nint.Zero);

    /// <summary>
    /// Assemble the specified assembly code at a base address.
    /// </summary>
    /// <param name="asm">The assembly code.</param>
    /// <param name="baseAddress">The address where the code is rebased.</param>
    /// <returns>An array of bytes containing the assembly code.</returns>
    public byte[] Assemble(string asm, nint baseAddress)
    {
        // Rebase the code
        asm = $"user32{Environment.NewLine}org 0x{baseAddress.ToInt64():X8}{Environment.NewLine}{asm}";
        // Assemble and return the code
        return Assemble(asm);
    }
}