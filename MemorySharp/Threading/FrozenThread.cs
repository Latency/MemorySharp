// ****************************************************************************
// Project:  MemorySharp
// File:     FrozenThread.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

namespace MemorySharp.Threading;

/// <summary>
///     Class containing a frozen thread. If an instance of this class is disposed, its associated thread is resumed.
/// </summary>
public class FrozenThread : IDisposable
{
    #region Constructor

    /// <summary>
    ///     Initializes a new instance of the <see cref="FrozenThread" /> class.
    /// </summary>
    /// <param name="thread">The frozen thread.</param>
    internal FrozenThread(RemoteThread thread) =>
        // Save the parameter
        Thread = thread;

    #endregion

    #region Properties

    /// <summary>
    ///     The frozen thread.
    /// </summary>
    public RemoteThread Thread { get; }

    #endregion

    #region Methods

    #region Dispose (implementation of IDisposable)

    /// <summary>
    ///     Releases all resources used by the <see cref="RemoteThread" /> object.
    /// </summary>
    public virtual void Dispose()
    {
        // Unfreeze the thread
        Thread.Resume();
    }

    #endregion

    #region ToString (override)

    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    public override string ToString() => $"Id = {Thread.Id}";

    #endregion

    #endregion
}