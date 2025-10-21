// ****************************************************************************
// Project:  Binarysharp
// File:     Extensions.cs
// Author:   Latency McLaughlin
// Date:     05/07/2025
// ****************************************************************************

using System.Diagnostics;

namespace MemorySharp.MemoryManagement.Native;

public static class Extensions
{
    public static void SuspendProcess(this Process process)
    {
        foreach (ProcessThread pT in process.Threads)
        {
            using var pOpenThread = NativeMethods.OpenThread(ThreadAccessFlags.SuspendResume, false, pT.Id);
            if (pOpenThread.DangerousGetHandle() == IntPtr.Zero)
                continue;

            NativeMethods.SuspendThread(pOpenThread);
        }
    }


    public static void ResumeProcess(this Process process)
    {
        foreach (ProcessThread pT in process.Threads)
        {
            using var pOpenThread = NativeMethods.OpenThread(ThreadAccessFlags.SuspendResume, false, pT.Id);
            if (pOpenThread.DangerousGetHandle() == IntPtr.Zero)
                continue;

            uint suspendCount;
            do
                suspendCount = NativeMethods.ResumeThread(pOpenThread);
            while (suspendCount > 0);
        }
    }
}