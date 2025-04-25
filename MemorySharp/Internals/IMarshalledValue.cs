// ****************************************************************************
// Project:  MemorySharp
// File:     IMarshalledValue.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

using MemorySharp.Memory;

namespace MemorySharp.Internals;

/// <summary>
///     Interface representing a value within the memory of a remote process.
/// </summary>
public interface IMarshalledValue : IDisposable
{
    /// <summary>
    ///     The memory allocated where the value is fully written if needed. It can be unused.
    /// </summary>
    RemoteAllocation Allocated { get; }

    /// <summary>
    ///     The reference of the value. It can be directly the value or a pointer.
    /// </summary>
    IntPtr Reference { get; }
}