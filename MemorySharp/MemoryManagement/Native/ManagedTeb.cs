/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using Binarysharp.Memory;
using Binarysharp.Threading;

namespace Binarysharp.MemoryManagement.Native;

/// <summary>
/// Class representing the Thread Environment Block of a remote thread.
/// </summary>
public class ManagedTeb : RemotePointer
{
    #region Properties
    /// <summary>
    /// Current Structured Exception Handling (SEH) frame.
    /// </summary>
    public nint CurrentSehFrame
    {
        get => Read<nint>(TebStructure.CurrentSehFrame);
        set => Write(TebStructure.CurrentSehFrame, value);
    }
    /// <summary>
    /// The top of stack.
    /// </summary>
    public nint TopOfStack
    {
        get => Read<nint>(TebStructure.TopOfStack);
        set => Write(TebStructure.TopOfStack, value);
    }
    /// <summary>
    /// The current bottom of stack.
    /// </summary>
    public nint BottomOfStack
    {
        get => Read<nint>(TebStructure.BottomOfStack);
        set => Write(TebStructure.BottomOfStack, value);
    }
    /// <summary>
    /// The TEB sub system.
    /// </summary>
    public nint SubSystemTeb
    {
        get => Read<nint>(TebStructure.SubSystemTeb);
        set => Write(TebStructure.SubSystemTeb, value);
    }
    /// <summary>
    /// The fiber data.
    /// </summary>
    public nint FiberData
    {
        get => Read<nint>(TebStructure.FiberData);
        set => Write(TebStructure.FiberData, value);
    }
    /// <summary>
    /// The arbitrary data slot.
    /// </summary>
    public nint ArbitraryDataSlot
    {
        get => Read<nint>(TebStructure.ArbitraryDataSlot);
        set => Write(TebStructure.ArbitraryDataSlot, value);
    }
    /// <summary>
    /// The linear address of Thread Environment Block (TEB).
    /// </summary>
    public nint Teb
    {
        get => Read<nint>(TebStructure.Teb);
        set => Write(TebStructure.Teb, value);
    }
    /// <summary>
    /// The environment pointer.
    /// </summary>
    public nint EnvironmentPointer
    {
        get => Read<nint>(TebStructure.EnvironmentPointer);
        set => Write(TebStructure.EnvironmentPointer, value);
    }
    /// <summary>
    /// The process Id.
    /// </summary>
    public int ProcessId
    {
        get => Read<int>(TebStructure.ProcessId);
        set => Write(TebStructure.ProcessId, value);
    }
    /// <summary>
    /// The current thread Id.
    /// </summary>
    public int ThreadId
    {
        get => Read<int>(TebStructure.ThreadId);
        set => Write(TebStructure.ThreadId, value);
    }
    /// <summary>
    /// The active RPC handle.
    /// </summary>
    public nint RpcHandle
    {
        get => Read<nint>(TebStructure.RpcHandle);
        set => Write(TebStructure.RpcHandle, value);
    }
    /// <summary>
    /// The linear address of the thread-local storage (TLS) array.
    /// </summary>
    public nint Tls
    {
        get => Read<nint>(TebStructure.Tls);
        set => Write(TebStructure.Tls, value);
    }
    /// <summary>
    /// The linear address of Process Environment Block (PEB).
    /// </summary>
    public nint Peb
    {
        get => Read<nint>(TebStructure.Peb);
        set => Write(TebStructure.Peb, value);
    }
    /// <summary>
    /// The last error number.
    /// </summary>
    public int LastErrorNumber
    {
        get => Read<int>(TebStructure.LastErrorNumber);
        set => Write(TebStructure.LastErrorNumber, value);
    }
    /// <summary>
    /// The count of owned critical sections.
    /// </summary>
    public int CriticalSectionsCount
    {
        get => Read<int>(TebStructure.CriticalSectionsCount);
        set => Write(TebStructure.CriticalSectionsCount, value);
    }
    /// <summary>
    /// The address of CSR Client Thread.
    /// </summary>
    public nint CsrClientThread
    {
        get => Read<nint>(TebStructure.CsrClientThread);
        set => Write(TebStructure.CsrClientThread, value);
    }
    /// <summary>
    /// Win32 Thread Information.
    /// </summary>
    public nint Win32ThreadInfo
    {
        get => Read<nint>(TebStructure.Win32ThreadInfo);
        set => Write(TebStructure.Win32ThreadInfo, value);
    }
    /// <summary>
    /// Win32 client information (NT), user32 private data (Wine), 0x60 = LastError (Win95), 0x74 = LastError (WinME).
    /// </summary>
    public byte[] Win32ClientInfo
    {
        get => Read<byte>(TebStructure.Win32ClientInfo, 124);
        set => Write(TebStructure.Win32ClientInfo, value);
    }
    /// <summary>
    /// Reserved for Wow64. Contains a pointer to FastSysCall in Wow64.
    /// </summary>
    public nint WoW64Reserved
    {
        get => Read<nint>(TebStructure.WoW64Reserved);
        set => Write(TebStructure.WoW64Reserved, value);
    }
    /// <summary>
    /// The current locale
    /// </summary>
    public nint CurrentLocale
    {
        get => Read<nint>(TebStructure.CurrentLocale);
        set => Write(TebStructure.CurrentLocale, value);
    }
    /// <summary>
    /// The FP Software Status Register.
    /// </summary>
    public nint FpSoftwareStatusRegister
    {
        get => Read<nint>(TebStructure.FpSoftwareStatusRegister);
        set => Write(TebStructure.FpSoftwareStatusRegister, value);
    }
    /// <summary>
    /// Reserved for OS (NT), kernel32 private data (Wine).
    /// herein: FS:[0x124] 4 NT Pointer to KTHREAD (ETHREAD) structure.
    /// </summary>
    public byte[] SystemReserved1
    {
        get => Read<byte>(TebStructure.SystemReserved1, 216);
        set => Write(TebStructure.SystemReserved1, value);
    }
    /// <summary>
    /// The exception code.
    /// </summary>
    public nint ExceptionCode
    {
        get => Read<nint>(TebStructure.ExceptionCode);
        set => Write(TebStructure.ExceptionCode, value);
    }
    /// <summary>
    /// The activation context stack.
    /// </summary>
    public byte[] ActivationContextStack
    {
        get => Read<byte>(TebStructure.ActivationContextStack, 18);
        set => Write(TebStructure.ActivationContextStack, value);
    }
    /// <summary>
    /// The spare bytes (NT), ntdll private data (Wine).
    /// </summary>
    public byte[] SpareBytes
    {
        get => Read<byte>(TebStructure.SpareBytes, 26);
        set => Write(TebStructure.SpareBytes, value);
    }
    /// <summary>
    /// Reserved for OS (NT), ntdll private data (Wine).
    /// </summary>
    public byte[] SystemReserved2
    {
        get => Read<byte>(TebStructure.SystemReserved2, 40);
        set => Write(TebStructure.SystemReserved2, value);
    }
    /// <summary>
    /// The GDI TEB Batch (OS), vm86 private data (Wine).
    /// </summary>
    public byte[] GdiTebBatch
    {
        get => Read<byte>(TebStructure.GdiTebBatch, 1248);
        set => Write(TebStructure.GdiTebBatch, value);
    }
    /// <summary>
    /// The GDI Region.
    /// </summary>
    public nint GdiRegion
    {
        get => Read<nint>(TebStructure.GdiRegion);
        set => Write(TebStructure.GdiRegion, value);
    }
    /// <summary>
    /// The GDI Pen.
    /// </summary>
    public nint GdiPen
    {
        get => Read<nint>(TebStructure.GdiPen);
        set => Write(TebStructure.GdiPen, value);
    }
    /// <summary>
    /// The GDI Brush.
    /// </summary>
    public nint GdiBrush
    {
        get => Read<nint>(TebStructure.GdiBrush);
        set => Write(TebStructure.GdiBrush, value);
    }
    /// <summary>
    /// The real process Id.
    /// </summary>
    public int RealProcessId
    {
        get => Read<int>(TebStructure.RealProcessId);
        set => Write(TebStructure.RealProcessId, value);
    }
    /// <summary>
    /// The real thread Id.
    /// </summary>
    public int RealThreadId
    {
        get => Read<int>(TebStructure.RealThreadId);
        set => Write(TebStructure.RealThreadId, value);
    }
    /// <summary>
    /// The GDI cached process handle.
    /// </summary>
    public nint GdiCachedProcessHandle
    {
        get => Read<nint>(TebStructure.GdiCachedProcessHandle);
        set => Write(TebStructure.GdiCachedProcessHandle, value);
    }
    /// <summary>
    /// The GDI client process Id (PID).
    /// </summary>
    public nint GdiClientProcessId
    {
        get => Read<nint>(TebStructure.GdiClientProcessId);
        set => Write(TebStructure.GdiClientProcessId, value);
    }
    /// <summary>
    /// The GDI client thread Id (TID).
    /// </summary>
    public nint GdiClientThreadId
    {
        get => Read<nint>(TebStructure.GdiClientThreadId);
        set => Write(TebStructure.GdiClientThreadId, value);
    }
    /// <summary>
    /// The GDI thread locale information.
    /// </summary>
    public nint GdiThreadLocalInfo
    {
        get => Read<nint>(TebStructure.GdiThreadLocalInfo);
        set => Write(TebStructure.GdiThreadLocalInfo, value);
    }
    /// <summary>
    /// Reserved for user application.
    /// </summary>
    public byte[] UserReserved1
    {
        get => Read<byte>(TebStructure.UserReserved1, 20);
        set => Write(TebStructure.UserReserved1, value);
    }
    /// <summary>
    /// Reserved for GL.
    /// </summary>
    public byte[] GlReserved1
    {
        get => Read<byte>(TebStructure.GlReserved1, 1248);
        set => Write(TebStructure.GlReserved1, value);
    }
    /// <summary>
    /// The last value status value.
    /// </summary>
    public int LastStatusValue
    {
        get => Read<int>(TebStructure.LastStatusValue);
        set => Write(TebStructure.LastStatusValue, value);
    }
    /// <summary>
    /// The static UNICODE_STRING buffer.
    /// </summary>
    public byte[] StaticUnicodeString
    {
        get => Read<byte>(TebStructure.StaticUnicodeString, 532);
        set => Write(TebStructure.StaticUnicodeString, value);
    }
    /// <summary>
    /// The pointer to deallocation stack.
    /// </summary>
    public nint DeallocationStack
    {
        get => Read<nint>(TebStructure.DeallocationStack);
        set => Write(TebStructure.DeallocationStack, value);
    }
    /// <summary>
    /// The TLS slots, 4 byte per slot.
    /// </summary>
    public nint[] TlsSlots
    {
        get => Read<nint>(TebStructure.TlsSlots, 64);
        set => Write(TebStructure.TlsSlots, value);
    }
    /// <summary>
    /// The TLS links (LIST_ENTRY structure).
    /// </summary>
    public long TlsLinks
    {
        get => Read<long>(TebStructure.TlsLinks);
        set => Write(TebStructure.TlsLinks, value);
    }
    /// <summary>
    /// Virtual DOS Machine.
    /// </summary>
    public nint Vdm
    {
        get => Read<nint>(TebStructure.Vdm);
        set => Write(TebStructure.Vdm, value);
    }
    /// <summary>
    /// Reserved for RPC.
    /// </summary>
    public nint RpcReserved
    {
        get => Read<nint>(TebStructure.RpcReserved);
        set => Write(TebStructure.RpcReserved, value);
    }
    /// <summary>
    /// The thread error mode (RtlSetThreadErrorMode).
    /// </summary>
    public nint ThreadErrorMode
    {
        get => Read<nint>(TebStructure.ThreadErrorMode);
        set => Write(TebStructure.ThreadErrorMode, value);
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedTeb"/> class.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp"/> object.</param>
    /// <param name="address">The location of the teb.</param>
    internal ManagedTeb(MemorySharp memorySharp, nint address) : base(memorySharp, address) {}
    #endregion

    #region Methods
    /// <summary>
    /// Finds the Thread Environment Block address of a specified thread.
    /// </summary>
    /// <param name="threadHandle">A handle of the thread.</param>
    /// <returns>A <see cref="nint"/> pointer of the TEB.</returns>
    public static nint FindTeb(SafeMemoryHandle threadHandle) => ThreadCore.NtQueryInformationThread(threadHandle).TebBaseAdress;

    #endregion
}