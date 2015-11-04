// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class SynthesizedStaticConstructor : MethodSymbol
    {
        private readonly NamedTypeSymbol _containingType;

        public SynthesizedStaticConstructor(NamedTypeSymbol containingType)
        {
            _containingType = containingType;
        }

        public override Symbol ContainingSymbol
        {
            get
            {
                return _containingType;
            }
        }

        public override NamedTypeSymbol ContainingType
        {
            get
            {
                return _containingType;
            }
        }

        public override string Name
        {
            get
            {
                return WellKnownMemberNames.StaticConstructorName;
            }
        }

        public override bool HasSpecialName
        {
            get { return true; }
        }

        public override System.Reflection.MethodImplAttributes ImplementationAttributes
        {
            get { return default(System.Reflection.MethodImplAttributes); }
        }

        public override bool IsVararg
        {
            get
            {
                return false;
            }
        }

        public override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get
            {
                return ImmutableArray<TypeParameterSymbol>.Empty;
            }
        }

        public override int ParameterCount
        {
            get
            {
                return 0;
            }
        }

        public override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                return ImmutableArray<ParameterSymbol>.Empty;
            }
        }

        public override bool TryGetThisParameter(out ParameterSymbol thisParameter)
        {
            thisParameter = null;
            return true;
        }

        public override Accessibility DeclaredAccessibility
        {
            get
            {
                //Same as for explicitly-declared static constructors
                //(see SourceConstructorSymbol.MakeModifiers)
                return Accessibility.Private;
            }
        }

        public override LexicalSortKey GetLexicalSortKey()
        {
            //For the sake of matching the metadata output of the native compiler, make synthesized constructors appear last in the metadata.
            //This is not critical, but it makes it easier on tools that are comparing metadata.
            return LexicalSortKey.SynthesizedCCtor;
        }

        public override ImmutableArray<Location> Locations
        {
            get
            {
                return ContainingType.Locations;
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                return ImmutableArray<SyntaxReference>.Empty;
            }
        }

        public override TypeSymbol ReturnType
        {
            get
            {
                return ContainingAssembly.GetSpecialType(SpecialType.System_Void);
            }
        }

        public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers
        {
            get
            {
                return ImmutableArray<CustomModifier>.Empty;
            }
        }

        public override ImmutableArray<TypeSymbol> TypeArguments
        {
            get
            {
                return ImmutableArray<TypeSymbol>.Empty;
            }
        }

        public override Symbol AssociatedSymbol
        {
            get
            {
                return null;
            }
        }

        public override int Arity
        {
            get
            {
                return 0;
            }
        }

        public override bool ReturnsVoid
        {
            get
            {
                return true;
            }
        }

        public override MethodKind MethodKind
        {
            get
            {
                return MethodKind.StaticConstructor;
            }
        }

        public override bool IsExtern
        {
            get
            {
                return false;
            }
        }

        public override bool IsSealed
        {
            get
            {
                return false;
            }
        }

        public override bool IsAbstract
        {
            get
            {
                return false;
            }
        }

        public override bool IsOverride
        {
            get
            {
                return false;
            }
        }

        public override bool IsVirtual
        {
            get
            {
                return false;
            }
        }

        public override bool IsStatic
        {
            get
            {
                return true;
            }
        }

        public override bool IsAsync
        {
            get
            {
                return false;
            }
        }

        public override bool HidesBaseMethodsByName
        {
            get
            {
                return false;
            }
        }

        public override bool IsExtensionMethod
        {
            get
            {
                return false;
            }
        }

        public override Microsoft.Cci.CallingConvention CallingConvention
        {
            get
            {
                //this matches the value in SourceMethodSymbol.CallingConvention for static methods
                return Microsoft.Cci.CallingConvention.Default;
            }
        }

        public override bool IsExplicitInterfaceImplementation
        {
            get { return false; }
        }

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
        {
            get
            {
                return ImmutableArray<MethodSymbol>.Empty;
            }
        }

        public sealed override bool IsImplicitlyDeclared
        {
            get
            {
                return true;
            }
        }

        public sealed override bool GenerateDebugInfo
        {
            get
            {
                // debugging static field initializers
                return true;
            }
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

        public override bool RequiresSecurityObject
        {
            get
            {
                return false;
            }
        }

        public override DllImportData GetDllImportData()
        {
            return null;
        }

        public override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation
        {
            get { return null; }
        }

        public override bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        public override IEnumerable<Microsoft.Cci.SecurityAttribute> GetSecurityInformation()
        {
            throw ExceptionUtilities.Unreachable;
        }

        public sealed override ObsoleteAttributeData ObsoleteAttributeData
        {
            get { return null; }
        }

        public override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        public override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            var containingType = (SourceMemberContainerTypeSymbol)this.ContainingType;
            return containingType.CalculateSyntaxOffsetInSynthesizedConstructor(localPosition, localTree, isStatic: true);
        }
    }
}
