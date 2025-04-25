// ****************************************************************************
// Project:  MemorySharp
// File:     WriteMemory.cs
// Author:   Latency McLaughlin
// Date:     05/12/2026
// ****************************************************************************

using MemorySharp.Memory;
using MemorySharp.Modules;
using System.Diagnostics;

namespace MemorySharp.Helpers;

public static class Extensions
{
    [DebuggerStepThrough]
    private static byte[] MakeDetour(nint address, nint destination)
    {
        var offset = (int)destination - (int)address;
        var patchBytes = new byte[]
        {
            0xE9,
            0x0,
            0x0,
            0x0,
            0x0
        }; // E9 = JMP
        BitConverter.GetBytes(offset).CopyTo(patchBytes, 1);
        return patchBytes;
    }


    // ReSharper disable once InconsistentNaming
    [DebuggerStepThrough]
    private static List<byte> NopsToDWORDs(int length)
    {
        var ary       = new List<byte>();
        var remainder = length;

        foreach (var x in new[]
        {
             9,
             7,
             6,
             5,
             2,
             1
        })
        {
            var dwords = remainder / x;
            remainder %= x;

            for (var y = 0; y < dwords; y++)
                Parser(x);
        }

        return ary;

        void Parser(int no)
        {
            Action foo = no switch
            {
                9 => () => ary.AddRange([0x66, 0x0F, 0x1F, 0x84, 0x00, 0x00, 0x00, 0x00, 0x00]), // 66 0F1F 84 00 00000000 - nop word  ptr [rax+rax+00000000]
                7 => () => ary.AddRange([0x0F, 0x1F, 0x80, 0x00, 0x00, 0x00, 0x00]),             // 0F1F 80 00000000       - nop dword ptr [rax+00000000]
                6 => () => ary.AddRange([0x66, 0x0F, 0x1F, 0x44, 0x00, 0x00]),                   // 66 0F1F 84 00 00       - nop word  ptr [rax+rax+00]
                5 => () => ary.AddRange([0x0F, 0x1F, 0x44, 0x00, 0x00]),                         // 0F1F 44 00 00          - nop dword ptr [rax+rax+00]
                2 => () => ary.AddRange([0x66, 0x90]),                                           // 66 90                  - nop 2
                _ => () => ary.AddRange([0x90])                                                  // 90                     - nop
            };
            foo.Invoke();
        }
    }


    private static nint WriteMemory(RemoteRegion region, nint offset, byte[] bytes1, byte[] bytes2)
    {
        var retAddress = offset;

        // If patch bytes[] > unpatch bytes[]
        if (bytes1.Length > bytes2.Length)
        {
            // Create a code cave
            var caveAddress     = MemoryCore.Allocate(region.MemorySharp.Handle, (uint)bytes1.Length);
            var relativeAddress = IntPtr.Add(region.BaseAddress, (int)offset);

            // Inject the code cave
            var bytes = MakeDetour(relativeAddress + 5, caveAddress);
            var nops  = NopsToDWORDs(bytes2.Length - 5);

            region.Write(Convert.ToInt32(offset), bytes.ToArray().Concat(nops).ToArray());

            // Layout the code cave.
            bytes = bytes1;

            // Jump back to original offset + [5 = Sizeof(JMP)]
            var a      = relativeAddress - bytes1.Length + nops.Count;
            var bytes3 = MakeDetour(caveAddress, a);
            var bytes4 = bytes.Concat(bytes3).ToArray();

            // Write bytes to the code cave
            region.MemorySharp.Write(caveAddress, bytes4, false);

            retAddress = caveAddress;
        }
        // ReSharper disable once InvertIf
        else if (bytes1.Length < bytes2.Length)
        {
            var length = bytes2.Length - bytes1.Length;
            var bytes  = new List<byte>(bytes1);
            // Append with NOPs
            var nops = NopsToDWORDs(length);
            bytes.AddRange(nops);
            region.Write(Convert.ToInt32(offset), bytes.ToArray());
        }
        else
        {
            // Write memory as-is
            region.Write(Convert.ToInt32(offset), bytes1);
        }

        return retAddress;
    }


    extension(RemoteRegion region)
    {
        public nint Patch(nint   offset, byte[] patchBytes, byte[] unpatchBytes) => WriteMemory(region, offset, patchBytes, unpatchBytes);
        public nint Unpatch(nint offset, byte[] patchBytes, byte[] unpatchBytes) => WriteMemory(region, offset, patchBytes, unpatchBytes);
    }
}