// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class WrappedParameterSymbol : ParameterSymbol
    {
        protected readonly ParameterSymbol underlyingParameter;

        protected WrappedParameterSymbol(ParameterSymbol underlyingParameter)
        {
            Debug.Assert((object)underlyingParameter != null);

            this.underlyingParameter = underlyingParameter;
        }

        public abstract override Symbol ContainingSymbol
        {
            get;
        }

        public override ParameterSymbol OriginalDefinition
        {
            get { return this; }
        }

        public sealed override bool Equals(object obj)
        {
            if ((object)this == obj)
            {
                return true;
            }

            // Equality of ordinal and containing symbol is a correct
            // implementation for all ParameterSymbols, but we don't 
            // define it on the base type because most can simply use
            // ReferenceEquals.

            var other = obj as WrappedParameterSymbol;
            return (object)other != null &&
                this.Ordinal == other.Ordinal &&
                this.ContainingSymbol.Equals(other.ContainingSymbol);
        }

        public sealed override int GetHashCode()
        {
            return Hash.Combine(ContainingSymbol, underlyingParameter.Ordinal);
        }

        #region Forwarded

        public override TypeSymbol Type
        {
            get { return underlyingParameter.Type; }
        }

        public sealed override RefKind RefKind
        {
            get { return underlyingParameter.RefKind; }
        }

        public sealed override bool IsMetadataIn
        {
            get { return underlyingParameter.IsMetadataIn; }
        }

        public sealed override bool IsMetadataOut
        {
            get { return underlyingParameter.IsMetadataOut; }
        }

        public sealed override ImmutableArray<Location> Locations
        {
            get { return underlyingParameter.Locations; }
        }

        public sealed override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { return underlyingParameter.DeclaringSyntaxReferences; }
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return underlyingParameter.GetAttributes();
        }

        public override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            underlyingParameter.AddSynthesizedAttributes(compilationState, ref attributes);
        }

        public sealed override ConstantValue ExplicitDefaultConstantValue
        {
            get { return underlyingParameter.ExplicitDefaultConstantValue; }
        }

        public override int Ordinal
        {
            get { return underlyingParameter.Ordinal; }
        }

        public override bool IsParams
        {
            get { return underlyingParameter.IsParams; }
        }

        public override bool IsMetadataOptional
        {
            get { return underlyingParameter.IsMetadataOptional; }
        }

        public override bool IsImplicitlyDeclared
        {
            get { return underlyingParameter.IsImplicitlyDeclared; }
        }

        public sealed override string Name
        {
            get { return underlyingParameter.Name; }
        }

        public override ImmutableArray<CustomModifier> CustomModifiers
        {
            get { return underlyingParameter.CustomModifiers; }
        }

        public override MarshalPseudoCustomAttributeData MarshallingInformation
        {
            get { return underlyingParameter.MarshallingInformation; }
        }

        public override UnmanagedType MarshallingType
        {
            get { return underlyingParameter.MarshallingType; }
        }

        public override bool IsIDispatchConstant
        {
            get { return underlyingParameter.IsIDispatchConstant; }
        }

        public override bool IsIUnknownConstant
        {
            get { return underlyingParameter.IsIUnknownConstant; }
        }

        public override bool IsCallerLineNumber
        {
            get { return underlyingParameter.IsCallerLineNumber; }
        }

        public override bool IsCallerFilePath
        {
            get { return underlyingParameter.IsCallerFilePath; }
        }

        public override bool IsCallerMemberName
        {
            get { return underlyingParameter.IsCallerMemberName; }
        }

        public sealed override ushort CountOfCustomModifiersPrecedingByRef
        {
            get { return underlyingParameter.CountOfCustomModifiersPrecedingByRef; }
        }

        #endregion
    }
}
