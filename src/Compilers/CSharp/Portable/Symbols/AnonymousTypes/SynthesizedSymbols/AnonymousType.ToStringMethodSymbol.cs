// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed partial class AnonymousTypeManager
    {
        /// <summary>
        /// Represents an anonymous type 'ToString' method.
        /// </summary>
        private sealed partial class AnonymousTypeToStringMethodSymbol : SynthesizedMethodBase
        {
            public AnonymousTypeToStringMethodSymbol(NamedTypeSymbol container)
                : base(container, WellKnownMemberNames.ObjectToString)
            {
            }

            public override MethodKind MethodKind
            {
                get { return MethodKind.Ordinary; }
            }

            public override bool ReturnsVoid
            {
                get { return false; }
            }

            public override TypeSymbol ReturnType
            {
                get { return this.Manager.System_String; }
            }

            public override ImmutableArray<ParameterSymbol> Parameters
            {
                get { return ImmutableArray<ParameterSymbol>.Empty; }
            }

            public override bool IsOverride
            {
                get { return true; }
            }

            public sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
            {
                return true;
            }

            public override bool IsMetadataFinal
            {
                get
                {
                    return false;
                }
            }
        }
    }
}
