/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using MemorySharp.Memory;

namespace MemorySharp.Native;

/// <summary>
/// Class representing the Process Environment Block of a remote process.
/// </summary>
public class ManagedPeb : RemotePointer
{
    #region Properties
    public byte InheritedAddressSpace
    {
        get => Read<byte>(PebStructure.InheritedAddressSpace);
        set => Write(PebStructure.InheritedAddressSpace, value);
    }
    public byte ReadImageFileExecOptions
    {
        get => Read<byte>(PebStructure.ReadImageFileExecOptions);
        set => Write(PebStructure.ReadImageFileExecOptions, value);
    }
    public bool BeingDebugged
    {
        get => Read<bool>(PebStructure.BeingDebugged);
        set => Write(PebStructure.BeingDebugged, value);
    }
    public byte SpareBool
    {
        get => Read<byte>(PebStructure.SpareBool);
        set => Write(PebStructure.SpareBool, value);
    }
    public IntPtr Mutant
    {
        get => Read<IntPtr>(PebStructure.Mutant);
        set => Write(PebStructure.Mutant, value);
    }
    public IntPtr Ldr
    {
        get => Read<IntPtr>(PebStructure.Ldr);
        set => Write(PebStructure.Ldr, value);
    }
    public IntPtr ProcessParameters
    {
        get => Read<IntPtr>(PebStructure.ProcessParameters);
        set => Write(PebStructure.ProcessParameters, value);
    }
    public IntPtr SubSystemData
    {
        get => Read<IntPtr>(PebStructure.SubSystemData);
        set => Write(PebStructure.SubSystemData, value);
    }
    public IntPtr ProcessHeap
    {
        get => Read<IntPtr>(PebStructure.ProcessHeap);
        set => Write(PebStructure.ProcessHeap, value);
    }
    public IntPtr FastPebLock
    {
        get => Read<IntPtr>(PebStructure.FastPebLock);
        set => Write(PebStructure.FastPebLock, value);
    }
    public IntPtr FastPebLockRoutine
    {
        get => Read<IntPtr>(PebStructure.FastPebLockRoutine);
        set => Write(PebStructure.FastPebLockRoutine, value);
    }
    public IntPtr FastPebUnlockRoutine
    {
        get => Read<IntPtr>(PebStructure.FastPebUnlockRoutine);
        set => Write(PebStructure.FastPebUnlockRoutine, value);
    }
    public IntPtr EnvironmentUpdateCount
    {
        get => Read<IntPtr>(PebStructure.EnvironmentUpdateCount);
        set => Write(PebStructure.EnvironmentUpdateCount, value);
    }
    public IntPtr KernelCallbackTable
    {
        get => Read<IntPtr>(PebStructure.KernelCallbackTable);
        set => Write(PebStructure.KernelCallbackTable, value);
    }
    public int SystemReserved
    {
        get => Read<int>(PebStructure.SystemReserved);
        set => Write(PebStructure.SystemReserved, value);
    }
    public int AtlThunkSListPtr32
    {
        get => Read<int>(PebStructure.AtlThunkSListPtr32);
        set => Write(PebStructure.AtlThunkSListPtr32, value);
    }
    public IntPtr FreeList
    {
        get => Read<IntPtr>(PebStructure.FreeList);
        set => Write(PebStructure.FreeList, value);
    }
    public IntPtr TlsExpansionCounter
    {
        get => Read<IntPtr>(PebStructure.TlsExpansionCounter);
        set => Write(PebStructure.TlsExpansionCounter, value);
    }
    public IntPtr TlsBitmap
    {
        get => Read<IntPtr>(PebStructure.TlsBitmap);
        set => Write(PebStructure.TlsBitmap, value);
    }
    public long TlsBitmapBits
    {
        get => Read<long>(PebStructure.TlsBitmapBits);
        set => Write(PebStructure.TlsBitmapBits, value);
    }
    public IntPtr ReadOnlySharedMemoryBase
    {
        get => Read<IntPtr>(PebStructure.ReadOnlySharedMemoryBase);
        set => Write(PebStructure.ReadOnlySharedMemoryBase, value);
    }
    public IntPtr ReadOnlySharedMemoryHeap
    {
        get => Read<IntPtr>(PebStructure.ReadOnlySharedMemoryHeap);
        set => Write(PebStructure.ReadOnlySharedMemoryHeap, value);
    }
    public IntPtr ReadOnlyStaticServerData
    {
        get => Read<IntPtr>(PebStructure.ReadOnlyStaticServerData);
        set => Write(PebStructure.ReadOnlyStaticServerData, value);
    }
    public IntPtr AnsiCodePageData
    {
        get => Read<IntPtr>(PebStructure.AnsiCodePageData);
        set => Write(PebStructure.AnsiCodePageData, value);
    }
    public IntPtr OemCodePageData
    {
        get => Read<IntPtr>(PebStructure.OemCodePageData);
        set => Write(PebStructure.OemCodePageData, value);
    }
    public IntPtr UnicodeCaseTableData
    {
        get => Read<IntPtr>(PebStructure.UnicodeCaseTableData);
        set => Write(PebStructure.UnicodeCaseTableData, value);
    }
    public int NumberOfProcessors
    {
        get => Read<int>(PebStructure.NumberOfProcessors);
        set => Write(PebStructure.NumberOfProcessors, value);
    }
    public long NtGlobalFlag
    {
        get => Read<long>(PebStructure.NtGlobalFlag);
        set => Write(PebStructure.NtGlobalFlag, value);
    }
    public long CriticalSectionTimeout
    {
        get => Read<long>(PebStructure.CriticalSectionTimeout);
        set => Write(PebStructure.CriticalSectionTimeout, value);
    }
    public IntPtr HeapSegmentReserve
    {
        get => Read<IntPtr>(PebStructure.HeapSegmentReserve);
        set => Write(PebStructure.HeapSegmentReserve, value);
    }
    public IntPtr HeapSegmentCommit
    {
        get => Read<IntPtr>(PebStructure.HeapSegmentCommit);
        set => Write(PebStructure.HeapSegmentCommit, value);
    }
    public IntPtr HeapDeCommitTotalFreeThreshold
    {
        get => Read<IntPtr>(PebStructure.HeapDeCommitTotalFreeThreshold);
        set => Write(PebStructure.HeapDeCommitTotalFreeThreshold, value);
    }
    public IntPtr HeapDeCommitFreeBlockThreshold
    {
        get => Read<IntPtr>(PebStructure.HeapDeCommitFreeBlockThreshold);
        set => Write(PebStructure.HeapDeCommitFreeBlockThreshold, value);
    }
    public int NumberOfHeaps
    {
        get => Read<int>(PebStructure.NumberOfHeaps);
        set => Write(PebStructure.NumberOfHeaps, value);
    }
    public int MaximumNumberOfHeaps
    {
        get => Read<int>(PebStructure.MaximumNumberOfHeaps);
        set => Write(PebStructure.MaximumNumberOfHeaps, value);
    }
    public IntPtr ProcessHeaps
    {
        get => Read<IntPtr>(PebStructure.ProcessHeaps);
        set => Write(PebStructure.ProcessHeaps, value);
    }
    public IntPtr GdiSharedHandleTable
    {
        get => Read<IntPtr>(PebStructure.GdiSharedHandleTable);
        set => Write(PebStructure.GdiSharedHandleTable, value);
    }
    public IntPtr ProcessStarterHelper
    {
        get => Read<IntPtr>(PebStructure.ProcessStarterHelper);
        set => Write(PebStructure.ProcessStarterHelper, value);
    }
    public IntPtr GdiDcAttributeList
    {
        get => Read<IntPtr>(PebStructure.GdiDcAttributeList);
        set => Write(PebStructure.GdiDcAttributeList, value);
    }
    public IntPtr LoaderLock
    {
        get => Read<IntPtr>(PebStructure.LoaderLock);
        set => Write(PebStructure.LoaderLock, value);
    }
    public int OsMajorVersion
    {
        get => Read<int>(PebStructure.OsMajorVersion);
        set => Write(PebStructure.OsMajorVersion, value);
    }
    public int OsMinorVersion
    {
        get => Read<int>(PebStructure.OsMinorVersion);
        set => Write(PebStructure.OsMinorVersion, value);
    }
    public ushort OsBuildNumber
    {
        get => Read<ushort>(PebStructure.OsBuildNumber);
        set => Write(PebStructure.OsBuildNumber, value);
    }
    public ushort OsCsdVersion
    {
        get => Read<ushort>(PebStructure.OsCsdVersion);
        set => Write(PebStructure.OsCsdVersion, value);
    }
    public int OsPlatformId
    {
        get => Read<int>(PebStructure.OsPlatformId);
        set => Write(PebStructure.OsPlatformId, value);
    }
    public int ImageSubsystem
    {
        get => Read<int>(PebStructure.ImageSubsystem);
        set => Write(PebStructure.ImageSubsystem, value);
    }
    public int ImageSubsystemMajorVersion
    {
        get => Read<int>(PebStructure.ImageSubsystemMajorVersion);
        set => Write(PebStructure.ImageSubsystemMajorVersion, value);
    }
    public IntPtr ImageSubsystemMinorVersion
    {
        get => Read<IntPtr>(PebStructure.ImageSubsystemMinorVersion);
        set => Write(PebStructure.ImageSubsystemMinorVersion, value);
    }
    public IntPtr ImageProcessAffinityMask
    {
        get => Read<IntPtr>(PebStructure.ImageProcessAffinityMask);
        set => Write(PebStructure.ImageProcessAffinityMask, value);
    }
    public IntPtr[] GdiHandleBuffer
    {
        get => Read<IntPtr>(PebStructure.GdiHandleBuffer, 0x22);
        set => Write(PebStructure.GdiHandleBuffer, value);
    }
    public IntPtr PostProcessInitRoutine
    {
        get => Read<IntPtr>(PebStructure.PostProcessInitRoutine);
        set => Write(PebStructure.PostProcessInitRoutine, value);
    }
    public IntPtr TlsExpansionBitmap
    {
        get => Read<IntPtr>(PebStructure.TlsExpansionBitmap);
        set => Write(PebStructure.TlsExpansionBitmap, value);
    }
    public IntPtr[] TlsExpansionBitmapBits
    {
        get => Read<IntPtr>(PebStructure.TlsExpansionBitmapBits, 0x20);
        set => Write(PebStructure.TlsExpansionBitmapBits, value);
    }
    public IntPtr SessionId
    {
        get => Read<IntPtr>(PebStructure.SessionId);
        set => Write(PebStructure.SessionId, value);
    }
    public long AppCompatFlags
    {
        get => Read<long>(PebStructure.AppCompatFlags);
        set => Write(PebStructure.AppCompatFlags, value);
    }
    public long AppCompatFlagsUser
    {
        get => Read<long>(PebStructure.AppCompatFlagsUser);
        set => Write(PebStructure.AppCompatFlagsUser, value);
    }
    public IntPtr ShimData
    {
        get => Read<IntPtr>(PebStructure.ShimData);
        set => Write(PebStructure.ShimData, value);
    }
    public IntPtr AppCompatInfo
    {
        get => Read<IntPtr>(PebStructure.AppCompatInfo);
        set => Write(PebStructure.AppCompatInfo, value);
    }
    public long CsdVersion
    {
        get => Read<long>(PebStructure.CsdVersion);
        set => Write(PebStructure.CsdVersion, value);
    }
    public IntPtr ActivationContextData
    {
        get => Read<IntPtr>(PebStructure.ActivationContextData);
        set => Write(PebStructure.ActivationContextData, value);
    }
    public IntPtr ProcessAssemblyStorageMap
    {
        get => Read<IntPtr>(PebStructure.ProcessAssemblyStorageMap);
        set => Write(PebStructure.ProcessAssemblyStorageMap, value);
    }
    public IntPtr SystemDefaultActivationContextData
    {
        get => Read<IntPtr>(PebStructure.SystemDefaultActivationContextData);
        set => Write(PebStructure.SystemDefaultActivationContextData, value);
    }
    public IntPtr SystemAssemblyStorageMap
    {
        get => Read<IntPtr>(PebStructure.SystemAssemblyStorageMap);
        set => Write(PebStructure.SystemAssemblyStorageMap, value);
    }
    public IntPtr MinimumStackCommit
    {
        get => Read<IntPtr>(PebStructure.MinimumStackCommit);
        set => Write(PebStructure.MinimumStackCommit, value);
    }
    #endregion

    #region Constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedPeb"/> class.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp"/> object.</param>
    /// <param name="address">The location of the peb.</param>
    internal ManagedPeb(MemorySharp memorySharp, IntPtr address) : base(memorySharp, address)
    {}
    #endregion

    #region Methods
    /// <summary>
    /// Finds the Process Environment Block address of a specified process.
    /// </summary>
    /// <param name="processHandle">A handle of the process.</param>
    /// <returns>A <see cref="IntPtr"/> pointer of the PEB.</returns>
    public static IntPtr FindPeb(SafeMemoryHandle processHandle) => MemoryCore.NtQueryInformationProcess(processHandle).PebBaseAddress;

    #endregion
}