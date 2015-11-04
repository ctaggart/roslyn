// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Reflection.PortableExecutable;

namespace Microsoft.Cci
{
    public sealed class NtHeader
    {
        // standard fields
        public PEMagic Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public int SizeOfCode;
        public int SizeOfInitializedData;
        public int SizeOfUninitializedData;
        public int AddressOfEntryPoint;
        public int BaseOfCode; // this.sectionHeaders[0].virtualAddress
        public int BaseOfData;

        // Windows

        public ulong ImageBase;
        public int SectionAlignment = 0x2000;
        public int FileAlignment;

        public ushort MajorOperatingSystemVersion = 4;
        public ushort MinorOperatingSystemVersion = 0;
        public ushort MajorImageVersion = 0;
        public ushort MinorImageVersion = 0;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;

        public int SizeOfImage;
        public int SizeOfHeaders;
        public uint Checksum = 0;

        public Subsystem Subsystem;
        public DllCharacteristics DllCharacteristics;

        public ulong SizeOfStackReserve;
        public ulong SizeOfStackCommit;
        public ulong SizeOfHeapReserve;
        public ulong SizeOfHeapCommit;

        public DirectoryEntry ExportTable;
        public DirectoryEntry ImportTable;
        public DirectoryEntry ResourceTable;
        public DirectoryEntry ExceptionTable;
        public DirectoryEntry CertificateTable;
        public DirectoryEntry BaseRelocationTable;
        public DirectoryEntry DebugTable;
        public DirectoryEntry CopyrightTable;
        public DirectoryEntry GlobalPointerTable;
        public DirectoryEntry ThreadLocalStorageTable;
        public DirectoryEntry LoadConfigTable;
        public DirectoryEntry BoundImportTable;
        public DirectoryEntry ImportAddressTable;
        public DirectoryEntry DelayImportTable;
        public DirectoryEntry CliHeaderTable;
    }
}
