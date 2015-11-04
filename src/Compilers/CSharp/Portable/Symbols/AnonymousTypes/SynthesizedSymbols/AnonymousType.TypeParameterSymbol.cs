// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Diagnostics;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed partial class AnonymousTypeManager
    {
        /// <summary>
        /// Represents an anonymous type template's type parameter.
        /// </summary>
        public sealed class AnonymousTypeParameterSymbol : TypeParameterSymbol
        {
            private readonly Symbol _container;
            private readonly int _ordinal;
            private readonly string _name;

            public AnonymousTypeParameterSymbol(Symbol container, int ordinal, string name)
            {
                Debug.Assert((object)container != null);
                Debug.Assert(!string.IsNullOrEmpty(name));

                _container = container;
                _ordinal = ordinal;
                _name = name;
            }

            public override TypeParameterKind TypeParameterKind
            {
                get
                {
                    return TypeParameterKind.Type;
                }
            }

            public override ImmutableArray<Location> Locations
            {
                get { return ImmutableArray<Location>.Empty; }
            }

            public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
            {
                get
                {
                    return ImmutableArray<SyntaxReference>.Empty;
                }
            }

            public override int Ordinal
            {
                get { return _ordinal; }
            }

            public override string Name
            {
                get { return _name; }
            }

            public override bool HasConstructorConstraint
            {
                get { return false; }
            }

            public override bool HasReferenceTypeConstraint
            {
                get { return false; }
            }

            public override bool HasValueTypeConstraint
            {
                get { return false; }
            }

            public override bool IsImplicitlyDeclared
            {
                get { return true; }
            }

            public override VarianceKind Variance
            {
                get { return VarianceKind.None; }
            }

            public override void EnsureAllConstraintsAreResolved()
            {
            }

            public override ImmutableArray<TypeSymbol> GetConstraintTypes(ConsList<TypeParameterSymbol> inProgress)
            {
                return ImmutableArray<TypeSymbol>.Empty;
            }

            public override Symbol ContainingSymbol
            {
                get { return _container; }
            }

            public override ImmutableArray<NamedTypeSymbol> GetInterfaces(ConsList<TypeParameterSymbol> inProgress)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            public override NamedTypeSymbol GetEffectiveBaseClass(ConsList<TypeParameterSymbol> inProgress)
            {
                return null;
            }

            public override TypeSymbol GetDeducedBaseType(ConsList<TypeParameterSymbol> inProgress)
            {
                return null;
            }
        }
    }
}
