// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// A source parameter that has no default value, no attributes,
    /// and is not params.
    /// </summary>
    public sealed class SourceSimpleParameterSymbol : SourceParameterSymbol
    {
        public SourceSimpleParameterSymbol(
            Symbol owner,
            TypeSymbol parameterType,
            int ordinal,
            RefKind refKind,
            string name,
            ImmutableArray<Location> locations)
            : base(owner, parameterType, ordinal, refKind, name, locations)
        {
        }

        public override ConstantValue ExplicitDefaultConstantValue
        {
            get { return null; }
        }

        public override bool IsMetadataOptional
        {
            get { return false; }
        }

        public override bool IsParams
        {
            get { return false; }
        }

        public override bool HasDefaultArgumentSyntax
        {
            get { return false; }
        }

        public override ImmutableArray<CustomModifier> CustomModifiers
        {
            get { return ImmutableArray<CustomModifier>.Empty; }
        }

        public override SyntaxReference SyntaxReference
        {
            get { return null; }
        }

        public override bool IsExtensionMethodThis
        {
            get { return false; }
        }

        public override bool IsMetadataIn
        {
            get { return false; }
        }

        public override bool IsMetadataOut
        {
            get { return RefKind == RefKind.Out; }
        }

        public override bool IsIDispatchConstant
        {
            get { return false; }
        }

        public override bool IsIUnknownConstant
        {
            get { return false; }
        }

        public override bool IsCallerFilePath
        {
            get { return false; }
        }

        public override bool IsCallerLineNumber
        {
            get { return false; }
        }

        public override bool IsCallerMemberName
        {
            get { return false; }
        }

        public override MarshalPseudoCustomAttributeData MarshallingInformation
        {
            get { return null; }
        }

        public override bool HasOptionalAttribute
        {
            get { return false; }
        }

        public override SyntaxList<AttributeListSyntax> AttributeDeclarationList
        {
            get { return default(SyntaxList<AttributeListSyntax>); }
        }

        public override CustomAttributesBag<CSharpAttributeData> GetAttributesBag()
        {
            state.NotePartComplete(CompletionPart.Attributes);
            return CustomAttributesBag<CSharpAttributeData>.Empty;
        }

        public override ConstantValue DefaultValueFromAttributes
        {
            get { return ConstantValue.NotAvailable; }
        }
    }
}
