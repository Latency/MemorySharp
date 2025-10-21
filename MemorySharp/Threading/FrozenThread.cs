/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
 */

namespace MemorySharp.Threading;

/// <summary>
/// Class containing a frozen thread. If an instance of this class is disposed, its associated thread is resumed.
/// </summary>
public class FrozenThread : IDisposable
{
    /// <summary>
    /// The frozen thread.
    /// </summary>
    public RemoteThread Thread { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FrozenThread"/> class.
    /// </summary>
    /// <param name="thread">The frozen thread.</param>
    internal FrozenThread(RemoteThread thread) => Thread = thread;

    /// <summary>
    /// Frees resources and perform other cleanup operations before it is reclaimed by garbage collection.
    /// </summary>
    ~FrozenThread() => Dispose();

    /// <summary>
    /// Releases all resources used by the <see cref="RemoteThread"/> object.
    /// </summary>
    public virtual void Dispose()
    {
        // Resume the thread.
        Thread.Resume();

        // Avoid the finalizer
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    public override string ToString() => $"Id = {Thread.Id}";
}