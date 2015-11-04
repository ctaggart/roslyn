// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis
{
    public abstract class SymbolFactory<ModuleSymbol, TypeSymbol>
        where TypeSymbol : class
    {
        public abstract TypeSymbol GetUnsupportedMetadataTypeSymbol(ModuleSymbol moduleSymbol, BadImageFormatException exception);

        /// <summary>
        /// Produce unbound generic type symbol if the type is a generic type.
        /// </summary>
        public abstract TypeSymbol MakeUnboundIfGeneric(ModuleSymbol moduleSymbol, TypeSymbol type);

        public abstract TypeSymbol GetSZArrayTypeSymbol(ModuleSymbol moduleSymbol, TypeSymbol elementType, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers);
        public abstract TypeSymbol GetMDArrayTypeSymbol(ModuleSymbol moduleSymbol, int rank, TypeSymbol elementType, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers,
                                                          ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds);

        /// <summary>
        /// Produce constructed type symbol.
        /// </summary>
        /// <param name="moduleSymbol"></param>
        /// <param name="generic">
        /// Symbol for generic type.
        /// </param>
        /// <param name="arguments">
        /// Generic type arguments, including those for containing types.
        /// </param>
        /// <param name="refersToNoPiaLocalType">
        /// Flags for arguments. Each item indicates whether corresponding argument refers to NoPia local types.
        /// </param>
        public abstract TypeSymbol SubstituteTypeParameters(ModuleSymbol moduleSymbol, TypeSymbol generic, ImmutableArray<KeyValuePair<TypeSymbol, ImmutableArray<ModifierInfo<TypeSymbol>>>> arguments, ImmutableArray<bool> refersToNoPiaLocalType);

        public abstract TypeSymbol GetByRefReturnTypeSymbol(ModuleSymbol moduleSymbol, TypeSymbol referencedType, ushort countOfCustomModifiersPrecedingByRef);

        public abstract TypeSymbol MakePointerTypeSymbol(ModuleSymbol moduleSymbol, TypeSymbol type, ImmutableArray<ModifierInfo<TypeSymbol>> customModifiers);
        public abstract TypeSymbol GetSpecialType(ModuleSymbol moduleSymbol, SpecialType specialType);
        public abstract TypeSymbol GetSystemTypeSymbol(ModuleSymbol moduleSymbol);
        public abstract TypeSymbol GetEnumUnderlyingType(ModuleSymbol moduleSymbol, TypeSymbol type);

        public abstract bool IsVolatileModifierType(ModuleSymbol moduleSymbol, TypeSymbol type);
        public abstract Cci.PrimitiveTypeCode GetPrimitiveTypeCode(ModuleSymbol moduleSymbol, TypeSymbol type);
    }
}
