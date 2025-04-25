// ****************************************************************************
// Project:  MemorySharp
// File:     CallingConventionSelector.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

using MemorySharp.Helpers;

namespace MemorySharp.Assembly.CallingConvention;

/// <summary>
///     Static class providing calling convention instances.
/// </summary>
public static class CallingConventionSelector
{
    /// <summary>
    ///     Gets a calling convention object according the given type.
    /// </summary>
    /// <param name="callingConvention">The type of calling convention to get.</param>
    /// <returns>The return value is a singleton of a <see cref="ICallingConvention" /> child.</returns>
    public static ICallingConvention Get(CallingConventions callingConvention) => callingConvention switch
    {
        CallingConventions.Cdecl             => Singleton<CdeclCallingConvention>.Instance,
        CallingConventions.Stdcall           => Singleton<StdcallCallingConvention>.Instance,
        CallingConventions.Fastcall          => Singleton<FastcallCallingConvention>.Instance,
        CallingConventions.GccThiscall       => Singleton<GccThiscallCallingConvention>.Instance,
        CallingConventions.MicrosoftThiscall => Singleton<MicrosoftThiscallCallingConvention>.Instance,
        CallingConventions.MicrosoftX64      => Singleton<MicrosoftX64CallingConvention>.Instance,
        _                                    => throw new ApplicationException("Unsupported calling convention.")
    };
}