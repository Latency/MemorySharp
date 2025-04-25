// ****************************************************************************
// Project:  MemorySharp
// File:     ICallingConvention.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

namespace MemorySharp.Assembly.CallingConvention;

/// <summary>
///     Interface defining a calling convention.
/// </summary>
public interface ICallingConvention
{
    /// <summary>
    ///     Formats a call to a function pointer.
    /// </summary>
    /// <param name="function">The function pointer.</param>
    /// <param name="parameters">The pointer of the parameters.</param>
    /// <param name="instructions">The list that receives the assembly instructions.</param>
    void FormatCall(IntPtr function, IntPtr[] parameters, List<string> instructions);
}