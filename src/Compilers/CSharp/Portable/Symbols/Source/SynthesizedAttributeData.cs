// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// Class to represent a synthesized attribute
    /// </summary>
    public sealed class SynthesizedAttributeData : SourceAttributeData
    {
        public SynthesizedAttributeData(MethodSymbol wellKnownMember, ImmutableArray<TypedConstant> arguments, ImmutableArray<KeyValuePair<String, TypedConstant>> namedArguments)
            : base(
            applicationNode: null,
            attributeClass: wellKnownMember.ContainingType,
            attributeConstructor: wellKnownMember,
            constructorArguments: arguments,
            constructorArgumentsSourceIndices: default(ImmutableArray<int>),
            namedArguments: namedArguments,
            hasErrors: false,
            isConditionallyOmitted: false)
        {
            Debug.Assert((object)wellKnownMember != null);
            Debug.Assert(!arguments.IsDefault);
            Debug.Assert(!namedArguments.IsDefault); // Frequently empty though.
        }
    }
}
