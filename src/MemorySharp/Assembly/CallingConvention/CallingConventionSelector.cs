/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using MemorySharp.Helpers;

namespace MemorySharp.Assembly.CallingConvention;

/// <summary>
/// Static class providing calling convention instances.
/// </summary>
public static class CallingConventionSelector
{
    /// <summary>
    /// Gets a calling convention object according the given type.
    /// </summary>
    /// <param name="callingConvention">The type of calling convention to get.</param>
    /// <returns>The return value is a singleton of a <see cref="ICallingConvention"/> child.</returns>
    public static ICallingConvention Get(CallingConventions callingConvention) => callingConvention switch
    {
        CallingConventions.Cdecl    => Singleton<CdeclCallingConvention>.Instance,
        CallingConventions.Stdcall  => Singleton<StdcallCallingConvention>.Instance,
        CallingConventions.Fastcall => Singleton<FastcallCallingConvention>.Instance,
        CallingConventions.Thiscall => Singleton<ThiscallCallingConvention>.Instance,
        _                           => throw new ApplicationException("Unsupported calling convention.")
    };
}