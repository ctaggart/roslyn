// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace Roslyn.Utilities
{
    public static class ImmutableArrayInterop
    {
        public static byte[] DangerousGetUnderlyingArray(this ImmutableArray<byte> array)
        {
            var union = new ByteArrayUnion();
            union.ImmutableArray = array;
            return union.MutableArray;
        }

        public static ImmutableArray<byte> DangerousToImmutableArray(ref byte[] array)
        {
            var union = new ByteArrayUnion();
            union.MutableArray = array;
            array = null;
            return union.ImmutableArray;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct ByteArrayUnion
        {
            [FieldOffset(0)]
            public byte[] MutableArray;

            [FieldOffset(0)]
            public ImmutableArray<byte> ImmutableArray;
        }
    }
}
