// ****************************************************************************
// Project:  MemorySharp
// File:     IAssembler.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

namespace MemorySharp.Assembly.Assembler;

/// <summary>
///     Interface defining an assembler.
/// </summary>
public interface IAssembler
{
    /// <summary>
    ///     Assemble the specified assembly code.
    /// </summary>
    /// <param name="instructions">The instructions represented in assembly code.</param>
    /// <returns>An array of bytes containing the assembly code.</returns>
    byte[] Assemble(IEnumerable<string> instructions);

    /// <summary>
    ///     Assemble the specified assembly code at a base address.
    /// </summary>
    /// <param name="instructions">The instructions represented in assembly code.</param>
    /// <param name="baseAddress">The address where the code is rebased.</param>
    /// <returns>An array of bytes containing the assembly code.</returns>
    byte[] Assemble(IEnumerable<string> instructions, IntPtr baseAddress);
}