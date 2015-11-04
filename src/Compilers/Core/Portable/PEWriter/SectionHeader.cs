// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Reflection.PortableExecutable;

namespace Microsoft.Cci
{
    // TODO: merge with System.Reflection.PortableExecutable.SectionHeader
    public sealed class SectionHeader
    {
        public readonly string Name;
        public readonly int VirtualSize;
        public readonly int RelativeVirtualAddress;
        public readonly int SizeOfRawData;
        public readonly int PointerToRawData;
        public readonly int PointerToRelocations;
        public readonly int PointerToLinenumbers;
        public readonly ushort NumberOfRelocations;
        public readonly ushort NumberOfLinenumbers;
        public readonly SectionCharacteristics Characteristics;

        public SectionHeader(
            string name,
            int virtualSize,
            int relativeVirtualAddress,
            int sizeOfRawData,
            int pointerToRawData,
            int pointerToRelocations,
            int pointerToLinenumbers,
            ushort numberOfRelocations,
            ushort numberOfLinenumbers,
            SectionCharacteristics characteristics)
        {
            Name = name;
            VirtualSize = virtualSize;
            RelativeVirtualAddress = relativeVirtualAddress;
            SizeOfRawData = sizeOfRawData;
            PointerToRawData = pointerToRawData;
            PointerToRelocations = pointerToRelocations;
            PointerToLinenumbers = pointerToLinenumbers;
            NumberOfRelocations = numberOfRelocations;
            NumberOfLinenumbers = numberOfLinenumbers;
            Characteristics = characteristics;
        }
    }
}
