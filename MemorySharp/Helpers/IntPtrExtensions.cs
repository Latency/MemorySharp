// ****************************************************************************
// Project:  MemorySharp
// File:     IntPtrExtensions.cs
// Author:   Latency McLaughlin
// Date:     05/10/2026
// ****************************************************************************

using System.Runtime.CompilerServices;

namespace MemorySharp.Helpers;

/// <summary>
///     Extensions methods for <see cref="IntPtr" />, in order to prevent to cast pointer to an architecture dependent
///     size.
/// </summary>
public static class IntPtrExtensions
{
    /// <param name="pointer">The pointer where the offset is added.</param>
    extension(IntPtr pointer)
    {
        /// <summary>
        ///     Adds a given offset to a pointer.
        /// </summary>
        /// <param name="offset">The offset to add.</param>
        /// <returns>The return value is a new instance of the class <see cref="IntPtr" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe IntPtr Add(IntPtr offset) => new((void*)(pointer.ToInt64() + offset.ToInt64()));

        /// <summary>
        ///     Determines whether two given pointers are equal.
        /// </summary>
        /// <param name="value">The right pointer.</param>
        /// <returns>The return value is <c>true</c> if the two pointers are equal; otherwise <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool IsEqual(int value) => (void*)pointer == (void*)value;

        /// <summary>
        ///     Indicates whether the value of the pointer is greater or equal to another one.
        /// </summary>
        /// <param name="other">The second pointer to compare.</param>
        /// <returns>
        ///     If the first pointer is greater or equal to the second one, the return value is <c>true</c>,
        ///     otherwise the return value is <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool IsGreaterOrEqualThan(IntPtr other) => (void*)pointer >= (void*)other;

        /// <summary>
        ///     Indicates whether the value of the pointer is greater to another one.
        /// </summary>
        /// <param name="other">The second pointer to compare.</param>
        /// <returns>
        ///     If the first pointer is greater to the second one, the return value is <c>true</c>,
        ///     otherwise the return value is <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool IsGreaterThan(IntPtr other) => (void*)pointer > (void*)other;

        /// <summary>
        ///     Indicates whether the value of the pointer is smaller to another one.
        /// </summary>
        /// <param name="other">The second pointer to compare.</param>
        /// <returns>
        ///     If the first pointer is smaller to the second one, the return value is <c>true</c>,
        ///     otherwise the return value is <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool IsSmallerThan(IntPtr other) => (void*)pointer < (void*)other;
    }
}