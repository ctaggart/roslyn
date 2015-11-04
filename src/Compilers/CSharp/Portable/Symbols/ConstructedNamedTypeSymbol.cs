// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// A named type symbol that results from substituting a new owner for a type declaration.
    /// </summary>
    public sealed class SubstitutedNestedTypeSymbol : SubstitutedNamedTypeSymbol
    {
        public SubstitutedNestedTypeSymbol(SubstitutedNamedTypeSymbol newContainer, NamedTypeSymbol originalDefinition)
            : base(
                newContainer: newContainer,
                map: newContainer.TypeSubstitution,
                originalDefinition: originalDefinition,
                // An Arity-0 member of an unbound type, e.g. A<>.B, is unbound.
                unbound: newContainer.IsUnboundGenericType && originalDefinition.Arity == 0)
        {
        }

        public override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics
        {
            get { return TypeParameters.Cast<TypeParameterSymbol, TypeSymbol>(); }
        }

        public override bool HasTypeArgumentsCustomModifiers
        {
            get
            {
                return false;
            }
        }

        public override ImmutableArray<ImmutableArray<CustomModifier>> TypeArgumentsCustomModifiers
        {
            get
            {
                return CreateEmptyTypeArgumentsCustomModifiers();
            }
        }

        public override NamedTypeSymbol ConstructedFrom
        {
            get { return this; }
        }
    }

    /// <summary>
    /// A generic named type symbol that has been constructed with type arguments distinct from its own type parameters.
    /// </summary>
    public sealed class ConstructedNamedTypeSymbol : SubstitutedNamedTypeSymbol
    {
        private readonly ImmutableArray<TypeSymbol> _typeArguments;
        private readonly bool _hasTypeArgumentsCustomModifiers;
        private readonly NamedTypeSymbol _constructedFrom;

        public ConstructedNamedTypeSymbol(NamedTypeSymbol constructedFrom, ImmutableArray<TypeWithModifiers> typeArguments, bool unbound = false)
            : base(newContainer: constructedFrom.ContainingSymbol,
                   map: new TypeMap(constructedFrom.ContainingType, constructedFrom.OriginalDefinition.TypeParameters, typeArguments),
                   originalDefinition: constructedFrom.OriginalDefinition,
                   constructedFrom: constructedFrom, unbound: unbound)
        {
            bool hasTypeArgumentsCustomModifiers = false;
            _typeArguments = typeArguments.SelectAsArray(a => 
                                                            {
                                                                if (!a.CustomModifiers.IsDefaultOrEmpty)
                                                                {
                                                                    hasTypeArgumentsCustomModifiers = true;
                                                                }

                                                                return a.Type;
                                                            });
            _hasTypeArgumentsCustomModifiers = hasTypeArgumentsCustomModifiers;
            _constructedFrom = constructedFrom;

            Debug.Assert(constructedFrom.Arity == typeArguments.Length);
            Debug.Assert(constructedFrom.Arity != 0);
        }

        public override NamedTypeSymbol ConstructedFrom
        {
            get
            {
                return _constructedFrom;
            }
        }

        public override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics
        {
            get
            {
                return _typeArguments;
            }
        }

        public override bool HasTypeArgumentsCustomModifiers
        {
            get
            {
                return _hasTypeArgumentsCustomModifiers;
            }
        }

        public override ImmutableArray<ImmutableArray<CustomModifier>> TypeArgumentsCustomModifiers
        {
            get
            {
                if (_hasTypeArgumentsCustomModifiers)
                {
                    return TypeSubstitution.GetTypeArgumentsCustomModifiersFor(_constructedFrom.OriginalDefinition);
                }

                return CreateEmptyTypeArgumentsCustomModifiers();
            }
        }

        public static bool TypeParametersMatchTypeArguments(ImmutableArray<TypeParameterSymbol> typeParameters, ImmutableArray<TypeWithModifiers> typeArguments)
        {
            int n = typeParameters.Length;
            Debug.Assert(typeArguments.Length == n);
            Debug.Assert(typeArguments.Length > 0);

            for (int i = 0; i < n; i++)
            {
                if (!typeArguments[i].Is(typeParameters[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public sealed override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            if (ConstructedFrom.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes) ||
                GetUnificationUseSiteDiagnosticRecursive(ref result, _typeArguments, owner, ref checkedTypes))
            {
                return true;
            }

            if (_hasTypeArgumentsCustomModifiers)
            {
                foreach (var modifiers in this.TypeArgumentsCustomModifiers)
                {
                    if (GetUnificationUseSiteDiagnosticRecursive(ref result, modifiers, owner, ref checkedTypes))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
