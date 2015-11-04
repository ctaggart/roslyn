// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;
using System;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class SynthesizedIntrinsicOperatorSymbol : MethodSymbol
    {
        private readonly TypeSymbol _containingType;
        private readonly string _name;
        private readonly ImmutableArray<ParameterSymbol> _parameters;
        private readonly TypeSymbol _returnType;
        private readonly bool _isCheckedBuiltin;

        public SynthesizedIntrinsicOperatorSymbol(TypeSymbol leftType, string name, TypeSymbol rightType, TypeSymbol returnType, bool isCheckedBuiltin)
        {
            if (leftType.Equals(rightType, ignoreCustomModifiersAndArraySizesAndLowerBounds: true))
            {
                _containingType = leftType;
            }
            else if (rightType.Equals(returnType, ignoreCustomModifiersAndArraySizesAndLowerBounds: true))
            {
                _containingType = rightType;
            }
            else
            {
                Debug.Assert(leftType.Equals(returnType, ignoreCustomModifiersAndArraySizesAndLowerBounds: true));
                _containingType = leftType;
            }

            _name = name;
            _returnType = returnType;

            Debug.Assert((leftType.IsDynamic() || rightType.IsDynamic()) == returnType.IsDynamic());
            Debug.Assert(_containingType.IsDynamic() == returnType.IsDynamic());

            _parameters = (new ParameterSymbol[] {new SynthesizedOperatorParameterSymbol(this, leftType, 0, "left"),
                                                      new SynthesizedOperatorParameterSymbol(this, rightType, 1, "right")}).AsImmutableOrNull();
            _isCheckedBuiltin = isCheckedBuiltin;
        }

        public SynthesizedIntrinsicOperatorSymbol(TypeSymbol container, string name, TypeSymbol returnType, bool isCheckedBuiltin)
        {
            _containingType = container;
            _name = name;
            _returnType = returnType;
            _parameters = (new ParameterSymbol[] { new SynthesizedOperatorParameterSymbol(this, container, 0, "value") }).AsImmutableOrNull();
            _isCheckedBuiltin = isCheckedBuiltin;
        }

        public override string Name
        {
            get
            {
                return _name;
            }
        }

        public override bool IsCheckedBuiltin
        {
            get
            {
                return _isCheckedBuiltin;
            }
        }

        public override MethodKind MethodKind
        {
            get
            {
                return MethodKind.BuiltinOperator;
            }
        }

        public override bool IsImplicitlyDeclared
        {
            get
            {
                return true;
            }
        }

        public override CSharpCompilation DeclaringCompilation
        {
            get
            {
                return null;
            }
        }

        public override string GetDocumentationCommentId()
        {
            return null;
        }

        public override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            return false;
        }

        public override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
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

        public override int Arity
        {
            get
            {
                return 0;
            }
        }

        public override bool IsExtensionMethod
        {
            get
            {
                return false;
            }
        }

        public override bool HasSpecialName
        {
            get
            {
                return true;
            }
        }

        public override System.Reflection.MethodImplAttributes ImplementationAttributes
        {
            get
            {
                return System.Reflection.MethodImplAttributes.Managed;
            }
        }

        public override bool HasDeclarativeSecurity
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

        public override IEnumerable<Cci.SecurityAttribute> GetSecurityInformation()
        {
            return SpecializedCollections.EmptyEnumerable<Cci.SecurityAttribute>();
        }

        public override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation
        {
            get
            {
                return null;
            }
        }

        public override bool RequiresSecurityObject
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

        public override bool IsVararg
        {
            get
            {
                return false;
            }
        }

        public override bool ReturnsVoid
        {
            get
            {
                return false;
            }
        }

        public override bool IsAsync
        {
            get
            {
                return false;
            }
        }

        public override TypeSymbol ReturnType
        {
            get
            {
                return _returnType;
            }
        }

        public override ImmutableArray<TypeSymbol> TypeArguments
        {
            get
            {
                return ImmutableArray<TypeSymbol>.Empty;
            }
        }

        public override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get
            {
                return ImmutableArray<TypeParameterSymbol>.Empty;
            }
        }

        public override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                return _parameters;
            }
        }

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
        {
            get
            {
                return ImmutableArray<MethodSymbol>.Empty;
            }
        }

        public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers
        {
            get
            {
                return ImmutableArray<CustomModifier>.Empty;
            }
        }

        public override Symbol AssociatedSymbol
        {
            get
            {
                return null;
            }
        }

        public override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        public override Cci.CallingConvention CallingConvention
        {
            get
            {
                return Cci.CallingConvention.Default;
            }
        }

        public override bool GenerateDebugInfo
        {
            get
            {
                return false;
            }
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
                return _containingType as NamedTypeSymbol;
            }
        }

        public override ImmutableArray<Location> Locations
        {
            get
            {
                return ImmutableArray<Location>.Empty;
            }
        }

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                return ImmutableArray<SyntaxReference>.Empty;
            }
        }

        public override Accessibility DeclaredAccessibility
        {
            get
            {
                return Accessibility.Public;
            }
        }

        public override bool IsStatic
        {
            get
            {
                return true;
            }
        }

        public override bool IsVirtual
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

        public override bool IsAbstract
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

        public override bool IsExtern
        {
            get
            {
                return false;
            }
        }

        public override ObsoleteAttributeData ObsoleteAttributeData
        {
            get
            {
                return null;
            }
        }

        public override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override bool Equals(object obj)
        {
            if (obj == (object)this)
            {
                return true;
            }

            var other = obj as SynthesizedIntrinsicOperatorSymbol;

            if ((object)other == null)
            {
                return false;
            }

            if (_isCheckedBuiltin == other._isCheckedBuiltin &&
                _parameters.Length == other._parameters.Length &&
                string.Equals(_name, other._name, StringComparison.Ordinal) &&
                _containingType == other._containingType &&
                _returnType == other._returnType)
            {
                for (int i = 0; i < _parameters.Length; i++)
                {
                    if (_parameters[i].Type != other._parameters[i].Type)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_name, Hash.Combine(_containingType, _parameters.Length));
        }

        private sealed class SynthesizedOperatorParameterSymbol : SynthesizedParameterSymbol
        {
            public SynthesizedOperatorParameterSymbol(
                SynthesizedIntrinsicOperatorSymbol container,
                TypeSymbol type,
                int ordinal,
                string name
            ) : base(container, type, ordinal, RefKind.None, name, ImmutableArray<CustomModifier>.Empty)

            {
            }

            public override bool Equals(object obj)
            {
                if (obj == (object)this)
                {
                    return true;
                }

                var other = obj as SynthesizedOperatorParameterSymbol;

                if ((object)other == null)
                {
                    return false;
                }

                return Ordinal == other.Ordinal && ContainingSymbol == other.ContainingSymbol;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(ContainingSymbol, Ordinal.GetHashCode());
            }
        }
    }
}
