// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.Cci
{
    /// <summary>
    /// Constants for producing and consuming streams of binary custom debug info.
    /// </summary>
    public static class CustomDebugInfoConstants
    {
        // The version number of the custom debug info binary format.
        // CDIVERSION in Dev10
        public const int CdiVersion = 4;

        // The number of bytes at the beginning of the byte array that contain global header information.
        // start after header (version byte + size byte + dword padding)
        public const int CdiGlobalHeaderSize = 4;

        // The number of bytes at the beginning of each custom debug info record that contain header information
        // common to all record types (i.e. byte, kind, size).
        // version byte + kind byte + two bytes padding + size dword
        public const int CdiRecordHeaderSize = 8;

        public const byte CdiKindUsingInfo = 0;
        public const byte CdiKindForwardInfo = 1;
        public const byte CdiKindForwardToModuleInfo = 2;
        public const byte CdiKindStateMachineHoistedLocalScopes = 3;
        public const byte CdiKindForwardIterator = 4;
        public const byte CdiKindDynamicLocals = 5;
        public const byte CdiKindEditAndContinueLocalSlotMap = 6;
        public const byte CdiKindEditAndContinueLambdaMap = 7;
    }
}
