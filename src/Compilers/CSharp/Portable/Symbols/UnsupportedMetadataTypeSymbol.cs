// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class UnsupportedMetadataTypeSymbol : ErrorTypeSymbol
    {
        private readonly BadImageFormatException _mrEx;

        public UnsupportedMetadataTypeSymbol(BadImageFormatException mrEx = null)
        {
            _mrEx = mrEx;
        }

        public override DiagnosticInfo ErrorInfo
        {
            get
            {
                return new CSDiagnosticInfo(ErrorCode.ERR_BogusType, string.Empty);
            }
        }

        public override bool MangleName
        {
            get
            {
                return false;
            }
        }
    }
}
