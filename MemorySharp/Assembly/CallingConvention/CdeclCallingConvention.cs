// ****************************************************************************
// Project:  MemorySharp
// File:     CdeclCallingConvention.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

namespace MemorySharp.Assembly.CallingConvention;

/// <summary>
///     Define the C Declaration Calling Convention.
/// </summary>
/// <remarks>
///     The cdecl is a calling convention that originates from the C programming language.
///     The parameter are pushed on the stack in the right-to-left order. The caller cleans the arguments from the stack.
///     The return value is stored in the register EAX.
/// </remarks>
public class CdeclCallingConvention : ICallingConvention
{
    /// <summary>
    ///     Formats a call to a function pointer.
    /// </summary>
    /// <param name="function">The function pointer.</param>
    /// <param name="parameters">The pointer of the parameters.</param>
    /// <param name="instructions">The list that receives the assembly instructions.</param>
    public void FormatCall(IntPtr function, IntPtr[] parameters, List<string> instructions)
    {
        instructions.AddRange(parameters.Reverse().Select(parameter => $"push {parameter}"));
        instructions.Add($"call {function}");
        instructions.Add($"add esp, {parameters.Length * 4}");
    }
}