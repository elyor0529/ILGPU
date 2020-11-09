// ---------------------------------------------------------------------------------------
//                                        ILGPU
//                        Copyright (c) 2016-2020 Marcel Koester
//                                    www.ilgpu.net
//
// File: LocalMemory.cs
//
// This file is part of ILGPU and is distributed under the University of Illinois Open
// Source License. See LICENSE.txt for details
// ---------------------------------------------------------------------------------------

using ILGPU.Frontend.Intrinsic;
using ILGPU.Runtime;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ILGPU
{
    /// <summary>
    /// Contains methods to allocate and manage local memory.
    /// </summary>
    public static class LocalMemory
    {
        /// <summary>
        /// A readonly reference to the <see cref="AllocateZero{T}(long)"/> method.
        /// </summary>
        private static readonly MethodInfo AllocateZeroMethod =
            typeof(LocalMemory).GetMethod(
                nameof(AllocateZero),
                BindingFlags.Public | BindingFlags.Static);

        /// <summary>
        /// Creates a typed <see cref="AllocateZero{T}(long)"/> method instance to invoke.
        /// </summary>
        /// <param name="elementType">The array element type.</param>
        /// <returns>The typed method instance.</returns>
        internal static MethodInfo GetAllocateZeroMethod(Type elementType) =>
            AllocateZeroMethod.MakeGenericMethod(elementType);

        /// <summary>
        /// Allocates a single element in shared memory.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <returns>An allocated element in shared memory.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [LocalMemoryIntrinsic(LocalMemoryIntrinsicKind.Allocate)]
        public static ArrayView<T> Allocate<T>(long extent)
            where T : unmanaged
        {
            Trace.Assert(extent >= 0, "Invalid extent");
            return new ArrayView<T>(
                UnmanagedMemoryViewSource.Create(
                    Interop.SizeOf<T>() * extent),
                0,
                extent);
        }

        /// <summary>
        /// Allocates a chunk of shared memory with the specified number of elements.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="extent">The extent (number of elements to allocate).</param>
        /// <returns>An allocated region of shared memory.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArrayView<T> AllocateZero<T>(long extent)
            where T : unmanaged
        {
            Trace.Assert(extent >= 0, "Invalid extent");
            var view = Allocate<T>(extent);
            for (long i = 0; i < extent; ++i)
                view[i] = default;
            return view;
        }
    }
}
