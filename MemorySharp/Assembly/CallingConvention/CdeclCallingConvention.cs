﻿/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System.Text;

namespace Binarysharp.Assembly.CallingConvention;

/// <summary>
/// Define the C Declaration Calling Convention.
/// </summary>
public class CdeclCallingConvention : ICallingConvention
{
    /// <summary>
    /// The name of the calling convention.
    /// </summary>
    public string Name => "Cdecl";

    /// <summary>
    /// Defines which function performs the clean-up task.
    /// </summary>
    public CleanupTypes Cleanup => CleanupTypes.Caller;

    /// <summary>
    /// Formats the given parameters to call a function.
    /// </summary>
    /// <param name="parameters">An array of parameters.</param>
    /// <returns>The mnemonics to pass the parameters.</returns>
    public string FormatParameters(nint[] parameters)
    {
        // Declare a var to store the mnemonics
        var ret = new StringBuilder();
        // For each parameters (in reverse order)
        parameters.Reverse();
        foreach (var parameter in parameters)
            ret.AppendLine($"push {parameter}");
        // Return the mnemonics
        return ret.ToString();
    }
    /// <summary>
    /// Formats the call of a given function.
    /// </summary>
    /// <param name="function">The function to call.</param>
    /// <returns>The mnemonics to call the function.</returns>
    public string FormatCalling(nint function) => $"call {function}";

    /// <summary>
    /// Formats the cleaning of a given number of parameters.
    /// </summary>
    /// <param name="nbParameters">The number of parameters to clean.</param>
    /// <returns>The mnemonics to clean a given number of parameters.</returns>
    public string FormatCleaning(int nbParameters) => $"add esp, {nbParameters * 4}";
}