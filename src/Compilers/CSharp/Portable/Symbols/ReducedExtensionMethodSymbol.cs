// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// An extension method with the "this" parameter removed.
    /// Used for the public binding API only, not for compilation.
    /// </summary>
    public sealed class ReducedExtensionMethodSymbol : MethodSymbol
    {
        private readonly MethodSymbol _reducedFrom;
        private readonly TypeMap _typeMap;
        private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;
        private readonly ImmutableArray<TypeSymbol> _typeArguments;
        private ImmutableArray<ParameterSymbol> _lazyParameters;

        /// <summary>
        /// Return the extension method in reduced form if the extension method
        /// is applicable, and satisfies type parameter constraints, based on the
        /// "this" argument type. Otherwise, returns null.
        /// </summary>
        public static MethodSymbol Create(MethodSymbol method, TypeSymbol receiverType, Compilation compilation)
        {
            Debug.Assert(method.IsExtensionMethod && method.MethodKind != MethodKind.ReducedExtension);
            Debug.Assert(method.ParameterCount > 0);
            Debug.Assert((object)receiverType != null);

            HashSet<DiagnosticInfo> useSiteDiagnostics = null;

            method = method.InferExtensionMethodTypeArguments(receiverType, compilation, ref useSiteDiagnostics);
            if ((object)method == null)
            {
                return null;
            }

            var conversions = new TypeConversions(method.ContainingAssembly.CorLibrary);
            var conversion = conversions.ConvertExtensionMethodThisArg(method.Parameters[0].Type, receiverType, ref useSiteDiagnostics);
            if (!conversion.Exists)
            {
                return null;
            }

            if (useSiteDiagnostics != null)
            {
                foreach (var diag in useSiteDiagnostics)
                {
                    if (diag.Severity == DiagnosticSeverity.Error)
                    {
                        return null;
                    }
                }
            }

            return Create(method);
        }

        public static MethodSymbol Create(MethodSymbol method)
        {
            Debug.Assert(method.IsExtensionMethod && method.MethodKind != MethodKind.ReducedExtension);

            // The reduced form is always created from the unconstructed method symbol.
            var constructedFrom = method.ConstructedFrom;
            var reducedMethod = new ReducedExtensionMethodSymbol(constructedFrom);

            if (constructedFrom == method)
            {
                return reducedMethod;
            }

            // If the given method is a constructed method, the same type arguments
            // are applied to construct the result from the reduced form.
            Debug.Assert(!method.TypeArguments.IsEmpty);
            return reducedMethod.Construct(method.TypeArguments);
        }

        private ReducedExtensionMethodSymbol(MethodSymbol reducedFrom)
        {
            Debug.Assert((object)reducedFrom != null);
            Debug.Assert(reducedFrom.IsExtensionMethod);
            Debug.Assert((object)reducedFrom.ReducedFrom == null);
            Debug.Assert(reducedFrom.ConstructedFrom == reducedFrom);
            Debug.Assert(reducedFrom.ParameterCount > 0);

            _reducedFrom = reducedFrom;
            _typeMap = TypeMap.Empty.WithAlphaRename(reducedFrom, this, out _typeParameters);
            _typeArguments = _typeMap.SubstituteTypesWithoutModifiers(reducedFrom.TypeArguments);
        }

        public override MethodSymbol CallsiteReducedFromMethod
        {
            get { return _reducedFrom.ConstructIfGeneric(_typeArguments); }
        }

        public override TypeSymbol ReceiverType
        {
            get
            {
                return _reducedFrom.Parameters[0].Type;
            }
        }

        public override TypeSymbol GetTypeInferredDuringReduction(TypeParameterSymbol reducedFromTypeParameter)
        {
            if ((object)reducedFromTypeParameter == null)
            {
                throw new System.ArgumentNullException();
            }

            if (reducedFromTypeParameter.ContainingSymbol != _reducedFrom)
            {
                throw new System.ArgumentException();
            }

            return null;
        }

        public override MethodSymbol ReducedFrom
        {
            get { return _reducedFrom; }
        }

        public override MethodSymbol ConstructedFrom
        {
            get
            {
                Debug.Assert(_reducedFrom.ConstructedFrom == _reducedFrom);
                return this;
            }
        }

        public override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get { return _typeParameters; }
        }

        public override ImmutableArray<TypeSymbol> TypeArguments
        {
            get { return _typeArguments; }
        }

        public override Microsoft.Cci.CallingConvention CallingConvention
        {
            get { return _reducedFrom.CallingConvention; }
        }

        public override int Arity
        {
            get { return _reducedFrom.Arity; }
        }

        public override string Name
        {
            get { return _reducedFrom.Name; }
        }

        public override bool HasSpecialName
        {
            get { return _reducedFrom.HasSpecialName; }
        }

        public override System.Reflection.MethodImplAttributes ImplementationAttributes
        {
            get { return _reducedFrom.ImplementationAttributes; }
        }

        public override bool RequiresSecurityObject
        {
            get { return _reducedFrom.RequiresSecurityObject; }
        }

        public override DllImportData GetDllImportData()
        {
            return _reducedFrom.GetDllImportData();
        }

        public override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation
        {
            get { return _reducedFrom.ReturnValueMarshallingInformation; }
        }

        public override bool HasDeclarativeSecurity
        {
            get { return _reducedFrom.HasDeclarativeSecurity; }
        }

        public override IEnumerable<Microsoft.Cci.SecurityAttribute> GetSecurityInformation()
        {
            return _reducedFrom.GetSecurityInformation();
        }

        public override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return _reducedFrom.GetAppliedConditionalSymbols();
        }

        public override AssemblySymbol ContainingAssembly
        {
            get { return _reducedFrom.ContainingAssembly; }
        }

        public override ImmutableArray<Location> Locations
        {
            get { return _reducedFrom.Locations; }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { return _reducedFrom.DeclaringSyntaxReferences; }
        }

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _reducedFrom.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
        }

        public override MethodSymbol OriginalDefinition
        {
            get { return this; }
        }

        public override bool IsExtern
        {
            get { return _reducedFrom.IsExtern; }
        }

        public override bool IsSealed
        {
            get { return _reducedFrom.IsSealed; }
        }

        public override bool IsVirtual
        {
            get { return _reducedFrom.IsVirtual; }
        }

        public override bool IsAbstract
        {
            get { return _reducedFrom.IsAbstract; }
        }

        public override bool IsOverride
        {
            get { return _reducedFrom.IsOverride; }
        }

        public override bool IsStatic
        {
            get { return false; }
        }

        public override bool IsAsync
        {
            get { return _reducedFrom.IsAsync; }
        }

        public override bool IsExtensionMethod
        {
            get { return true; }
        }

        public sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        public sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        public override bool IsMetadataFinal
        {
            get
            {
                return false;
            }
        }

        public sealed override ObsoleteAttributeData ObsoleteAttributeData
        {
            get { return _reducedFrom.ObsoleteAttributeData; }
        }

        public override Accessibility DeclaredAccessibility
        {
            get { return _reducedFrom.DeclaredAccessibility; }
        }

        public override Symbol ContainingSymbol
        {
            get { return _reducedFrom.ContainingSymbol; }
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return _reducedFrom.GetAttributes();
        }

        public override Symbol AssociatedSymbol
        {
            get { return null; }
        }

        public override MethodKind MethodKind
        {
            get { return MethodKind.ReducedExtension; }
        }

        public override bool ReturnsVoid
        {
            get { return _reducedFrom.ReturnsVoid; }
        }

        public override bool IsGenericMethod
        {
            get { return _reducedFrom.IsGenericMethod; }
        }

        public override bool IsVararg
        {
            get { return _reducedFrom.IsVararg; }
        }

        public override TypeSymbol ReturnType
        {
            get { return _typeMap.SubstituteType(_reducedFrom.ReturnType).Type; }
        }

        public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers
        {
            get { return _typeMap.SubstituteCustomModifiers(_reducedFrom.ReturnType, _reducedFrom.ReturnTypeCustomModifiers); }
        }

        public override int ParameterCount
        {
            get { return _reducedFrom.ParameterCount - 1; }
        }

        public override bool GenerateDebugInfo
        {
            get { return _reducedFrom.GenerateDebugInfo; }
        }

        public override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                if (_lazyParameters.IsDefault)
                {
                    ImmutableInterlocked.InterlockedCompareExchange(ref _lazyParameters, this.MakeParameters(), default(ImmutableArray<ParameterSymbol>));
                }
                return _lazyParameters;
            }
        }

        public override bool IsExplicitInterfaceImplementation
        {
            get { return false; }
        }

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
        {
            get { return ImmutableArray<MethodSymbol>.Empty; }
        }

        public override bool HidesBaseMethodsByName
        {
            get { return false; }
        }

        public override bool CallsAreOmitted(SyntaxTree syntaxTree)
        {
            return _reducedFrom.CallsAreOmitted(syntaxTree);
        }

        private ImmutableArray<ParameterSymbol> MakeParameters()
        {
            var reducedFromParameters = _reducedFrom.Parameters;
            int count = reducedFromParameters.Length;

            if (count <= 1)
            {
                Debug.Assert(count == 1);
                return ImmutableArray<ParameterSymbol>.Empty;
            }
            else
            {
                var parameters = new ParameterSymbol[count - 1];
                for (int i = 0; i < count - 1; i++)
                {
                    parameters[i] = new ReducedExtensionMethodParameterSymbol(this, reducedFromParameters[i + 1]);
                }

                return parameters.AsImmutableOrNull();
            }
        }

        public override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override bool Equals(object obj)
        {
            if ((object)this == obj) return true;

            ReducedExtensionMethodSymbol other = obj as ReducedExtensionMethodSymbol;
            return (object)other != null && _reducedFrom.Equals(other._reducedFrom);
        }

        public override int GetHashCode()
        {
            return _reducedFrom.GetHashCode();
        }

        private sealed class ReducedExtensionMethodParameterSymbol : WrappedParameterSymbol
        {
            private readonly ReducedExtensionMethodSymbol _containingMethod;

            public ReducedExtensionMethodParameterSymbol(ReducedExtensionMethodSymbol containingMethod, ParameterSymbol underlyingParameter) :
                base(underlyingParameter)
            {
                Debug.Assert(underlyingParameter.Ordinal > 0);
                _containingMethod = containingMethod;
            }

            public override Symbol ContainingSymbol
            {
                get { return _containingMethod; }
            }

            public override int Ordinal
            {
                get { return this.underlyingParameter.Ordinal - 1; }
            }

            public override TypeSymbol Type
            {
                get { return _containingMethod._typeMap.SubstituteType(this.underlyingParameter.Type).Type; }
            }

            public override ImmutableArray<CustomModifier> CustomModifiers
            {
                get
                {
                    return _containingMethod._typeMap.SubstituteCustomModifiers(this.underlyingParameter.Type, this.underlyingParameter.CustomModifiers);
                }
            }
        }
    }
}
