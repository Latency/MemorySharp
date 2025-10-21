/*
 * MemorySharp Library
 * http://www.binarysharp.com/
 *
 * Copyright (C) 2012-2016 Jämes Ménétrey (a.k.a. ZenLulz).
 * This library is released under the MIT License.
 * See the file LICENSE for more information.
*/

using MemorySharp.Memory;

namespace MemorySharp.MemoryManagement.Native;

/// <summary>
/// Class representing the Process Environment Block of a remote process.
/// </summary>
public class ManagedPeb : RemotePointer
{
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
    public nint Mutant
    {
        get => Read<nint>(PebStructure.Mutant);
        set => Write(PebStructure.Mutant, value);
    }
    public nint Ldr
    {
        get => Read<nint>(PebStructure.Ldr);
        set => Write(PebStructure.Ldr, value);
    }
    public nint ProcessParameters
    {
        get => Read<nint>(PebStructure.ProcessParameters);
        set => Write(PebStructure.ProcessParameters, value);
    }
    public nint SubSystemData
    {
        get => Read<nint>(PebStructure.SubSystemData);
        set => Write(PebStructure.SubSystemData, value);
    }
    public nint ProcessHeap
    {
        get => Read<nint>(PebStructure.ProcessHeap);
        set => Write(PebStructure.ProcessHeap, value);
    }
    public nint FastPebLock
    {
        get => Read<nint>(PebStructure.FastPebLock);
        set => Write(PebStructure.FastPebLock, value);
    }
    public nint FastPebLockRoutine
    {
        get => Read<nint>(PebStructure.FastPebLockRoutine);
        set => Write(PebStructure.FastPebLockRoutine, value);
    }
    public nint FastPebUnlockRoutine
    {
        get => Read<nint>(PebStructure.FastPebUnlockRoutine);
        set => Write(PebStructure.FastPebUnlockRoutine, value);
    }
    public nint EnvironmentUpdateCount
    {
        get => Read<nint>(PebStructure.EnvironmentUpdateCount);
        set => Write(PebStructure.EnvironmentUpdateCount, value);
    }
    public nint KernelCallbackTable
    {
        get => Read<nint>(PebStructure.KernelCallbackTable);
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
    public nint FreeList
    {
        get => Read<nint>(PebStructure.FreeList);
        set => Write(PebStructure.FreeList, value);
    }
    public nint TlsExpansionCounter
    {
        get => Read<nint>(PebStructure.TlsExpansionCounter);
        set => Write(PebStructure.TlsExpansionCounter, value);
    }
    public nint TlsBitmap
    {
        get => Read<nint>(PebStructure.TlsBitmap);
        set => Write(PebStructure.TlsBitmap, value);
    }
    public long TlsBitmapBits
    {
        get => Read<long>(PebStructure.TlsBitmapBits);
        set => Write(PebStructure.TlsBitmapBits, value);
    }
    public nint ReadOnlySharedMemoryBase
    {
        get => Read<nint>(PebStructure.ReadOnlySharedMemoryBase);
        set => Write(PebStructure.ReadOnlySharedMemoryBase, value);
    }
    public nint ReadOnlySharedMemoryHeap
    {
        get => Read<nint>(PebStructure.ReadOnlySharedMemoryHeap);
        set => Write(PebStructure.ReadOnlySharedMemoryHeap, value);
    }
    public nint ReadOnlyStaticServerData
    {
        get => Read<nint>(PebStructure.ReadOnlyStaticServerData);
        set => Write(PebStructure.ReadOnlyStaticServerData, value);
    }
    public nint AnsiCodePageData
    {
        get => Read<nint>(PebStructure.AnsiCodePageData);
        set => Write(PebStructure.AnsiCodePageData, value);
    }
    public nint OemCodePageData
    {
        get => Read<nint>(PebStructure.OemCodePageData);
        set => Write(PebStructure.OemCodePageData, value);
    }
    public nint UnicodeCaseTableData
    {
        get => Read<nint>(PebStructure.UnicodeCaseTableData);
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
    public nint HeapSegmentReserve
    {
        get => Read<nint>(PebStructure.HeapSegmentReserve);
        set => Write(PebStructure.HeapSegmentReserve, value);
    }
    public nint HeapSegmentCommit
    {
        get => Read<nint>(PebStructure.HeapSegmentCommit);
        set => Write(PebStructure.HeapSegmentCommit, value);
    }
    public nint HeapDeCommitTotalFreeThreshold
    {
        get => Read<nint>(PebStructure.HeapDeCommitTotalFreeThreshold);
        set => Write(PebStructure.HeapDeCommitTotalFreeThreshold, value);
    }
    public nint HeapDeCommitFreeBlockThreshold
    {
        get => Read<nint>(PebStructure.HeapDeCommitFreeBlockThreshold);
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
    public nint ProcessHeaps
    {
        get => Read<nint>(PebStructure.ProcessHeaps);
        set => Write(PebStructure.ProcessHeaps, value);
    }
    public nint GdiSharedHandleTable
    {
        get => Read<nint>(PebStructure.GdiSharedHandleTable);
        set => Write(PebStructure.GdiSharedHandleTable, value);
    }
    public nint ProcessStarterHelper
    {
        get => Read<nint>(PebStructure.ProcessStarterHelper);
        set => Write(PebStructure.ProcessStarterHelper, value);
    }
    public nint GdiDcAttributeList
    {
        get => Read<nint>(PebStructure.GdiDcAttributeList);
        set => Write(PebStructure.GdiDcAttributeList, value);
    }
    public nint LoaderLock
    {
        get => Read<nint>(PebStructure.LoaderLock);
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
    public nint ImageSubsystemMinorVersion
    {
        get => Read<nint>(PebStructure.ImageSubsystemMinorVersion);
        set => Write(PebStructure.ImageSubsystemMinorVersion, value);
    }
    public nint ImageProcessAffinityMask
    {
        get => Read<nint>(PebStructure.ImageProcessAffinityMask);
        set => Write(PebStructure.ImageProcessAffinityMask, value);
    }
    public nint[] GdiHandleBuffer
    {
        get => Read<nint>(PebStructure.GdiHandleBuffer, 0x22);
        set => Write(PebStructure.GdiHandleBuffer, value);
    }
    public nint PostProcessInitRoutine
    {
        get => Read<nint>(PebStructure.PostProcessInitRoutine);
        set => Write(PebStructure.PostProcessInitRoutine, value);
    }
    public nint TlsExpansionBitmap
    {
        get => Read<nint>(PebStructure.TlsExpansionBitmap);
        set => Write(PebStructure.TlsExpansionBitmap, value);
    }
    public nint[] TlsExpansionBitmapBits
    {
        get => Read<nint>(PebStructure.TlsExpansionBitmapBits, 0x20);
        set => Write(PebStructure.TlsExpansionBitmapBits, value);
    } public nint SessionId
    {
        get => Read<nint>(PebStructure.SessionId);
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
    public nint ShimData
    {
        get => Read<nint>(PebStructure.ShimData);
        set => Write(PebStructure.ShimData, value);
    }
    public nint AppCompatInfo
    {
        get => Read<nint>(PebStructure.AppCompatInfo);
        set => Write(PebStructure.AppCompatInfo, value);
    }
    public long CsdVersion
    {
        get => Read<long>(PebStructure.CsdVersion);
        set => Write(PebStructure.CsdVersion, value);
    }
    public nint ActivationContextData
    {
        get => Read<nint>(PebStructure.ActivationContextData);
        set => Write(PebStructure.ActivationContextData, value);
    }
    public nint ProcessAssemblyStorageMap
    {
        get => Read<nint>(PebStructure.ProcessAssemblyStorageMap);
        set => Write(PebStructure.ProcessAssemblyStorageMap, value);
    }
    public nint SystemDefaultActivationContextData
    {
        get => Read<nint>(PebStructure.SystemDefaultActivationContextData);
        set => Write(PebStructure.SystemDefaultActivationContextData, value);
    }
    public nint SystemAssemblyStorageMap
    {
        get => Read<nint>(PebStructure.SystemAssemblyStorageMap);
        set => Write(PebStructure.SystemAssemblyStorageMap, value);
    }
    public nint MinimumStackCommit
    {
        get => Read<nint>(PebStructure.MinimumStackCommit);
        set => Write(PebStructure.MinimumStackCommit, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ManagedPeb"/> class.
    /// </summary>
    /// <param name="memorySharp">The reference of the <see cref="MemorySharp"/> object.</param>
    /// <param name="address">The location of the peb.</param>
    internal ManagedPeb(MemorySharp memorySharp, nint address) : base(memorySharp, address)
    {}

    /// <summary>
    /// Finds the Process Environment Block address of a specified process.
    /// </summary>
    /// <param name="processHandle">A handle of the process.</param>
    /// <returns>A <see cref="nint"/> pointer of the PEB.</returns>
    public static nint FindPeb(SafeMemoryHandle processHandle) => MemoryCore.NtQueryInformationProcess(processHandle).PebBaseAddress;
}