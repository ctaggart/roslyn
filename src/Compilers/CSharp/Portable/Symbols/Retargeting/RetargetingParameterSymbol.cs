// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Emit;

namespace Microsoft.CodeAnalysis.CSharp.Symbols.Retargeting
{
    /// <summary>
    /// Represents a parameter of a RetargetingMethodSymbol. Essentially this is a wrapper around 
    /// another ParameterSymbol that is responsible for retargeting symbols from one assembly to another. 
    /// It can retarget symbols for multiple assemblies at the same time.
    /// </summary>
    public abstract class RetargetingParameterSymbol : ParameterSymbol
    {
        private readonly ParameterSymbol _underlyingParameter;
        private ImmutableArray<CustomModifier> _lazyCustomModifiers;

        /// <summary>
        /// Retargeted custom attributes
        /// </summary>
        private ImmutableArray<CSharpAttributeData> _lazyCustomAttributes;

        protected RetargetingParameterSymbol(ParameterSymbol underlyingParameter)
        {
            Debug.Assert(!(underlyingParameter is RetargetingParameterSymbol));
            _underlyingParameter = underlyingParameter;
        }

        // test only
        public ParameterSymbol UnderlyingParameter
        {
            get
            {
                return _underlyingParameter;
            }
        }

        protected abstract RetargetingModuleSymbol RetargetingModule
        {
            get;
        }

        public sealed override TypeSymbol Type
        {
            get
            {
                return this.RetargetingModule.RetargetingTranslator.Retarget(_underlyingParameter.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode);
            }
        }

        public sealed override ImmutableArray<CustomModifier> CustomModifiers
        {
            get
            {
                return RetargetingModule.RetargetingTranslator.RetargetModifiers(
                    _underlyingParameter.CustomModifiers,
                    ref _lazyCustomModifiers);
            }
        }

        public sealed override Symbol ContainingSymbol
        {
            get
            {
                return this.RetargetingModule.RetargetingTranslator.Retarget(_underlyingParameter.ContainingSymbol);
            }
        }

        public sealed override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return this.RetargetingModule.RetargetingTranslator.GetRetargetedAttributes(_underlyingParameter.GetAttributes(), ref _lazyCustomAttributes);
        }

        public sealed override IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
        {
            return this.RetargetingModule.RetargetingTranslator.RetargetAttributes(_underlyingParameter.GetCustomAttributesToEmit(compilationState));
        }

        public sealed override AssemblySymbol ContainingAssembly
        {
            get
            {
                return this.RetargetingModule.ContainingAssembly;
            }
        }

        public sealed override ModuleSymbol ContainingModule
        {
            get
            {
                return this.RetargetingModule;
            }
        }

        public sealed override bool HasMetadataConstantValue
        {
            get
            {
                return _underlyingParameter.HasMetadataConstantValue;
            }
        }

        public sealed override bool IsMarshalledExplicitly
        {
            get
            {
                return _underlyingParameter.IsMarshalledExplicitly;
            }
        }

        public override MarshalPseudoCustomAttributeData MarshallingInformation
        {
            get
            {
                return this.RetargetingModule.RetargetingTranslator.Retarget(_underlyingParameter.MarshallingInformation);
            }
        }

        public override ImmutableArray<byte> MarshallingDescriptor
        {
            get
            {
                return _underlyingParameter.MarshallingDescriptor;
            }
        }

        public sealed override CSharpCompilation DeclaringCompilation // perf, not correctness
        {
            get { return null; }
        }

        #region Forwarded

        public sealed override ConstantValue ExplicitDefaultConstantValue
        {
            get { return _underlyingParameter.ExplicitDefaultConstantValue; }
        }

        public sealed override RefKind RefKind
        {
            get { return _underlyingParameter.RefKind; }
        }

        public sealed override bool IsMetadataIn
        {
            get { return _underlyingParameter.IsMetadataIn; }
        }

        public sealed override bool IsMetadataOut
        {
            get { return _underlyingParameter.IsMetadataOut; }
        }

        public sealed override ImmutableArray<Location> Locations
        {
            get { return _underlyingParameter.Locations; }
        }

        public sealed override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { return _underlyingParameter.DeclaringSyntaxReferences; }
        }

        public override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            _underlyingParameter.AddSynthesizedAttributes(compilationState, ref attributes);
        }

        public override int Ordinal
        {
            get { return _underlyingParameter.Ordinal; }
        }

        public override bool IsParams
        {
            get { return _underlyingParameter.IsParams; }
        }

        public override bool IsMetadataOptional
        {
            get { return _underlyingParameter.IsMetadataOptional; }
        }

        public override bool IsImplicitlyDeclared
        {
            get { return _underlyingParameter.IsImplicitlyDeclared; }
        }

        public sealed override string Name
        {
            get { return _underlyingParameter.Name; }
        }

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _underlyingParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
        }

        public sealed override UnmanagedType MarshallingType
        {
            get { return _underlyingParameter.MarshallingType; }
        }

        public sealed override bool IsIDispatchConstant
        {
            get { return _underlyingParameter.IsIDispatchConstant; }
        }

        public sealed override bool IsIUnknownConstant
        {
            get { return _underlyingParameter.IsIUnknownConstant; }
        }

        public sealed override bool IsCallerLineNumber
        {
            get { return _underlyingParameter.IsCallerLineNumber; }
        }

        public sealed override bool IsCallerFilePath
        {
            get { return _underlyingParameter.IsCallerFilePath; }
        }

        public sealed override bool IsCallerMemberName
        {
            get { return _underlyingParameter.IsCallerMemberName; }
        }

        public sealed override ushort CountOfCustomModifiersPrecedingByRef
        {
            get { return _underlyingParameter.CountOfCustomModifiersPrecedingByRef; }
        }

        #endregion
    }

    public sealed class RetargetingMethodParameterSymbol : RetargetingParameterSymbol
    {
        /// <summary>
        /// Owning RetargetingMethodSymbol.
        /// </summary>
        private readonly RetargetingMethodSymbol _retargetingMethod;

        public RetargetingMethodParameterSymbol(RetargetingMethodSymbol retargetingMethod, ParameterSymbol underlyingParameter)
            : base(underlyingParameter)
        {
            Debug.Assert((object)retargetingMethod != null);
            _retargetingMethod = retargetingMethod;
        }

        protected override RetargetingModuleSymbol RetargetingModule
        {
            get { return _retargetingMethod.RetargetingModule; }
        }
    }

    public sealed class RetargetingPropertyParameterSymbol : RetargetingParameterSymbol
    {
        /// <summary>
        /// Owning RetargetingPropertySymbol.
        /// </summary>
        private readonly RetargetingPropertySymbol _retargetingProperty;

        public RetargetingPropertyParameterSymbol(RetargetingPropertySymbol retargetingProperty, ParameterSymbol underlyingParameter)
            : base(underlyingParameter)
        {
            Debug.Assert((object)retargetingProperty != null);
            _retargetingProperty = retargetingProperty;
        }

        protected override RetargetingModuleSymbol RetargetingModule
        {
            get { return _retargetingProperty.RetargetingModule; }
        }
    }
}
