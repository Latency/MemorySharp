/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;
using MemorySharp.Internals;
using MemorySharp.Memory;
using MemorySharp.MemoryManagement.Native;

namespace MemorySharp.Threading;

/// <summary>
/// Class providing tools for manipulating threads.
/// </summary>
public class ThreadFactory : IFactory
{
    /// <summary>
    /// The reference of the <see cref="MemorySharp"/> object.
    /// </summary>
    protected readonly MemoryManagement.MemorySharp MemorySharp;

    /// <summary>
    /// Gets the main thread of the remote process.
    /// </summary>
    [SupportedOSPlatform("Windows")]
    [SupportedOSPlatform("Linux")]
    public RemoteThread MainThread => new(MemorySharp, NativeThreads.Aggregate((current, next) => next.StartTime < current.StartTime ? next : current));

    /// <summary>
    /// Gets the native threads from the remote process.
    /// </summary>
    internal IEnumerable<ProcessThread> NativeThreads
    {
        get
        {
            // Refresh the process info
            MemorySharp.Native.Refresh();
            // Enumerates all threads
            return MemorySharp.Native.Threads.Cast<ProcessThread>();
        }
    }

    /// <summary>
    /// Gets the threads from the remote process.
    /// </summary>
    public IEnumerable<RemoteThread> RemoteThreads => NativeThreads.Select(t => new RemoteThread(MemorySharp, t));

    /// <summary>
    /// Gets the thread corresponding to an id.
    /// </summary>
    /// <param name="threadId">The unique identifier of the thread to get.</param>
    /// <returns>A new instance of a <see cref="RemoteThread"/> class.</returns>
    public RemoteThread this[int threadId] => new(MemorySharp, NativeThreads.First(t => t.Id == threadId));

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadFactory"/> class.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp"/> object.</param>
    internal ThreadFactory(MemoryManagement.MemorySharp memorySharp) => MemorySharp = memorySharp;

    /// <summary>
    /// Creates a thread that runs in the remote process.
    /// </summary>
    /// <param name="address">
    /// A pointer to the application-defined function to be executed by the thread and represents
    /// the starting address of the thread in the remote process.
    /// </param>
    /// <param name="parameter">A variable to be passed to the thread function.</param>
    /// <param name="isStarted">Sets if the thread must be started just after being created.</param>
    /// <returns>A new instance of the <see cref="RemoteThread"/> class.</returns>
    public async Task<RemoteThread> Create(nint address, dynamic parameter, bool isStarted = true)
    {
        // alocating some memory on the target process - enough to store the name of the dll and storing its address in a pointer
        var allocMemAddress = MemoryCore.Allocate(MemorySharp.Handle, (uint)parameter.Length);

        // writing the name of the dll there
        MemorySharp.Write(allocMemAddress, Encoding.Default.GetBytes(parameter), false);

        // creating a thread that will call LoadLibraryA with allocMemAddress as argument
        var tuple = ThreadCore.CreateRemoteThread(MemorySharp.Handle, address, allocMemAddress, ThreadCreationFlags.Run);

        Trace.WriteLine($"[*] Thread ID: {tuple.Item1}");

        // Get the native thread previously created & loop until the native thread is retrieved
        RemoteThread? result;
        do
        {
            result = MemorySharp.Threads.RemoteThreads.FirstOrDefault(x => x.Id == tuple.Item2);
            if (result is null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500)).ConfigureAwait(false);
                MemorySharp.Native.Refresh();
            }
        } while (result is null);

        // If the thread must be started
        if (isStarted)
            result.Resume();

        return result;
    }

    /// <summary>
    /// Creates a thread that runs in the remote process.
    /// </summary>
    /// <param name="address">
    /// A pointer to the application-defined function to be executed by the thread and represents
    /// the starting address of the thread in the remote process.
    /// </param>
    /// <param name="isStarted">Sets if the thread must be started just after being created.</param>
    /// <returns>A new instance of the <see cref="RemoteThread"/> class.</returns>
    public RemoteThread Create(nint address, bool isStarted = true)
    {
        var tuple = ThreadCore.CreateRemoteThread(MemorySharp.Handle, address, nint.Zero, ThreadCreationFlags.Suspended);

        // Create the thread
        var ret = ThreadCore.NtQueryInformationThread(tuple.Item1);

        // Get the native thread previously created
        // Loop until the native thread is retrieved
        ProcessThread? nativeThread;
        do
        {
            nativeThread = MemorySharp.Threads.NativeThreads.FirstOrDefault(t => t.Id == ret.ThreadId);
        } while (nativeThread == null);

        // Wrap the native thread in an object of the library
        var result = new RemoteThread(MemorySharp, nativeThread);

        // If the thread must be started
        if (isStarted)
            result.Resume();
        return result;
    }

    /// <summary>
    /// Creates a thread in the remote process and blocks the calling thread until the thread terminates.
    /// </summary>
    /// <param name="address">
    /// A pointer to the application-defined function to be executed by the thread and represents
    /// the starting address of the thread in the remote process.
    /// </param>
    /// <param name="parameter">A variable to be passed to the thread function.</param>
    /// <returns>A new instance of the <see cref="RemoteThread"/> class.</returns>
    public RemoteThread CreateAndJoin(nint address, dynamic parameter)
    {
        // Create the thread
        var ret = Create(address, parameter);
        // Wait the end of the thread
        ret.Join();
        // Return the thread
        return ret;
    }

    /// <summary>
    /// Creates a thread in the remote process and blocks the calling thread until the thread terminates.
    /// </summary>
    /// <param name="address">
    /// A pointer to the application-defined function to be executed by the thread and represents
    /// the starting address of the thread in the remote process.
    /// </param>
    /// <returns>A new instance of the <see cref="RemoteThread"/> class.</returns>
    public RemoteThread CreateAndJoin(nint address)
    {
        // Create the thread
        var ret = Create(address);
        // Wait the end of the thread
        ret.Join();
        // Return the thread
        return ret;
    }

    /// <summary>
    /// Releases all resources used by the <see cref="ThreadFactory"/> object.
    /// </summary>
    public void Dispose()
    {
        // Nothing to dispose... yet

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets a thread by its id in the remote process.
    /// </summary>
    /// <param name="id">The id of the thread.</param>
    /// <returns>A new instance of the <see cref="RemoteThread"/> class.</returns>
    public RemoteThread GetThreadById(int id) => new(MemorySharp, NativeThreads.First(t => t.Id == id));

    /// <summary>
    /// Resumes all threads.
    /// </summary>
    public void ResumeAll()
    {
        foreach (var thread in RemoteThreads)
            thread.Resume();
    }

    /// <summary>
    /// Suspends all threads.
    /// </summary>
    public void SuspendAll()
    {
        foreach (var thread in RemoteThreads)
            thread.Suspend();
    }
}