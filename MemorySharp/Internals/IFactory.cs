// ****************************************************************************
// Project:  MemorySharp
// File:     IFactory.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

namespace MemorySharp.Internals;

/// <summary>
///     Define a factory for the library.
/// </summary>
/// <remarks>At the moment, the factories are just disposable.</remarks>
public interface IFactory : IDisposable
{ }