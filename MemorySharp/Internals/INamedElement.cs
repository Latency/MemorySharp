// ****************************************************************************
// Project:  MemorySharp
// File:     INamedElement.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

namespace MemorySharp.Internals;

/// <summary>
///     Defines a element with a name.
/// </summary>
public interface INamedElement : IApplicableElement
{
    /// <summary>
    ///     The name of the element.
    /// </summary>
    string Name { get; }
}