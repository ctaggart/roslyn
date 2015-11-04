// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.Emit;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed partial class AnonymousTypeManager
    {
        private sealed class NameAndIndex
        {
            public NameAndIndex(string name, int index)
            {
                this.Name = name;
                this.Index = index;
            }

            public readonly string Name;
            public readonly int Index;
        }

        /// <summary>
        /// Represents an anonymous type 'template' which is a generic type to be used for all 
        /// anonymous type having the same structure, i.e. the same number of fields and field names.
        /// </summary>
        private sealed class AnonymousTypeTemplateSymbol : NamedTypeSymbol
        {
            /// <summary> Name to be used as metadata name during emit </summary>
            private NameAndIndex _nameAndIndex;

            private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;
            private readonly ImmutableArray<Symbol> _members;

            /// <summary> This list consists of synthesized method symbols for ToString, 
            /// Equals and GetHashCode which are not part of symbol table </summary>
            public readonly ImmutableArray<MethodSymbol> SpecialMembers;

            /// <summary> Properties defined in the template </summary>
            public readonly ImmutableArray<AnonymousTypePropertySymbol> Properties;

            /// <summary> Maps member names to symbol(s) </summary>
            private readonly MultiDictionary<string, Symbol> _nameToSymbols = new MultiDictionary<string, Symbol>();

            /// <summary> Anonymous type manager owning this template </summary>
            public readonly AnonymousTypeManager Manager;

            /// <summary> Smallest location of the template, actually contains the smallest location 
            /// of all the anonymous type instances created using this template during EMIT </summary>
            private Location _smallestLocation;

            /// <summary> Key pf the anonymous type descriptor </summary>
            public readonly string TypeDescriptorKey;

            public AnonymousTypeTemplateSymbol(AnonymousTypeManager manager, AnonymousTypeDescriptor typeDescr)
            {
                this.Manager = manager;
                this.TypeDescriptorKey = typeDescr.Key;
                _smallestLocation = typeDescr.Location;

                // Will be set when the type's metadata is ready to be emitted, 
                // <anonymous-type>.Name will throw exception if requested
                // before that moment.
                _nameAndIndex = null;

                int fieldsCount = typeDescr.Fields.Length;

                // members
                Symbol[] members = new Symbol[fieldsCount * 3 + 1];
                int memberIndex = 0;

                // The array storing property symbols to be used in 
                // generation of constructor and other methods
                if (fieldsCount > 0)
                {
                    AnonymousTypePropertySymbol[] propertiesArray = new AnonymousTypePropertySymbol[fieldsCount];
                    TypeParameterSymbol[] typeParametersArray = new TypeParameterSymbol[fieldsCount];

                    // Process fields
                    for (int fieldIndex = 0; fieldIndex < fieldsCount; fieldIndex++)
                    {
                        AnonymousTypeField field = typeDescr.Fields[fieldIndex];

                        // Add a type parameter
                        AnonymousTypeParameterSymbol typeParameter =
                            new AnonymousTypeParameterSymbol(this, fieldIndex, GeneratedNames.MakeAnonymousTypeParameterName(field.Name));
                        typeParametersArray[fieldIndex] = typeParameter;

                        // Add a property
                        AnonymousTypePropertySymbol property = new AnonymousTypePropertySymbol(this, field, typeParameter);
                        propertiesArray[fieldIndex] = property;

                        // Property related symbols
                        members[memberIndex++] = property;
                        members[memberIndex++] = property.BackingField;
                        members[memberIndex++] = property.GetMethod;
                    }

                    _typeParameters = typeParametersArray.AsImmutable();
                    this.Properties = propertiesArray.AsImmutable();
                }
                else
                {
                    _typeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
                    this.Properties = ImmutableArray<AnonymousTypePropertySymbol>.Empty;
                }

                // Add a constructor
                members[memberIndex++] = new AnonymousTypeConstructorSymbol(this, this.Properties);
                _members = members.AsImmutable();

                Debug.Assert(memberIndex == _members.Length);

                // fill nameToSymbols map
                foreach (var symbol in _members)
                {
                    _nameToSymbols.Add(symbol.Name, symbol);
                }

                // special members: Equals, GetHashCode, ToString
                MethodSymbol[] specialMembers = new MethodSymbol[3];
                specialMembers[0] = new AnonymousTypeEqualsMethodSymbol(this);
                specialMembers[1] = new AnonymousTypeGetHashCodeMethodSymbol(this);
                specialMembers[2] = new AnonymousTypeToStringMethodSymbol(this);
                this.SpecialMembers = specialMembers.AsImmutable();
            }

            public AnonymousTypeKey GetAnonymousTypeKey()
            {
                var properties = this.Properties.SelectAsArray(p => new AnonymousTypeKeyField(p.Name, isKey: false, ignoreCase: false));
                return new AnonymousTypeKey(properties);
            }

            /// <summary>
            /// Smallest location of the template, actually contains the smallest location 
            /// of all the anonymous type instances created using this template during EMIT;
            /// 
            /// NOTE: if this property is queried, smallest location must not be null.
            /// </summary>
            public Location SmallestLocation
            {
                get
                {
                    Debug.Assert(_smallestLocation != null);
                    return _smallestLocation;
                }
            }

            public NameAndIndex NameAndIndex
            {
                get
                {
                    return _nameAndIndex;
                }
                set
                {
                    var oldValue = Interlocked.CompareExchange(ref _nameAndIndex, value, null);
                    Debug.Assert(oldValue == null ||
                        ((oldValue.Name == value.Name) && (oldValue.Index == value.Index)));
                }
            }

            /// <summary>
            /// In emit phase every time a created anonymous type is referenced we try to store the lowest 
            /// location of the template. It will be used for ordering templates and assigning emitted type names.
            /// </summary>
            public void AdjustLocation(Location location)
            {
                Debug.Assert(location.IsInSource);

                while (true)
                {
                    // Loop until we managed to set location OR we detected that we don't need to set it 
                    // in case 'location' in type descriptor is bigger that the one in smallestLocation

                    Location currentSmallestLocation = _smallestLocation;
                    if (currentSmallestLocation != null && this.Manager.Compilation.CompareSourceLocations(currentSmallestLocation, location) < 0)
                    {
                        // The template's smallest location do not need to be changed
                        return;
                    }

                    if (ReferenceEquals(Interlocked.CompareExchange(ref _smallestLocation, location, currentSmallestLocation), currentSmallestLocation))
                    {
                        // Changed successfully, proceed to updating the fields
                        return;
                    }
                }
            }

            public override ImmutableArray<Symbol> GetMembers()
            {
                return _members;
            }

            public override IEnumerable<FieldSymbol> GetFieldsToEmit()
            {
                foreach (var m in this.GetMembers())
                {
                    switch (m.Kind)
                    {
                        case SymbolKind.Field:
                            yield return (FieldSymbol)m;
                            break;
                    }
                }
            }

            public override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics
            {
                get { return StaticCast<TypeSymbol>.From(this.TypeParameters); }
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

            public override ImmutableArray<Symbol> GetMembers(string name)
            {
                var symbols = _nameToSymbols[name];
                var builder = ArrayBuilder<Symbol>.GetInstance(symbols.Count);
                foreach (var symbol in symbols)
                {
                    builder.Add(symbol);
                }

                return builder.ToImmutableAndFree();
            }

            public override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers()
            {
                return this.GetMembersUnordered();
            }

            public override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers(string name)
            {
                return this.GetMembers(name);
            }

            public override IEnumerable<string> MemberNames
            {
                get { return _nameToSymbols.Keys; }
            }

            public override Symbol ContainingSymbol
            {
                get { return this.Manager.Compilation.SourceModule.GlobalNamespace; }
            }

            public override string Name
            {
                get { return _nameAndIndex.Name; }
            }

            public override bool HasSpecialName
            {
                get { return false; }
            }

            public override bool MangleName
            {
                get { return this.Arity > 0; }
            }

            public override int Arity
            {
                get { return _typeParameters.Length; }
            }

            public override bool IsImplicitlyDeclared
            {
                get { return true; }
            }

            public override ImmutableArray<TypeParameterSymbol> TypeParameters
            {
                get { return _typeParameters; }
            }

            public override bool IsAbstract
            {
                get { return false; }
            }

            public override bool IsSealed
            {
                get { return true; }
            }

            public override bool MightContainExtensionMethods
            {
                get { return false; }
            }

            public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            public override Accessibility DeclaredAccessibility
            {
                get { return Accessibility.Internal; }
            }

            public override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<Symbol> basesBeingResolved)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            public override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            public override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics
            {
                get { return this.Manager.System_Object; }
            }

            public override TypeKind TypeKind
            {
                get { return TypeKind.Class; }
            }

            public override bool IsInterface
            {
                get { return false; }
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

            public override bool IsStatic
            {
                get { return false; }
            }

            public override NamedTypeSymbol ConstructedFrom
            {
                get { return this; }
            }

            public override NamedTypeSymbol GetDeclaredBaseType(ConsList<Symbol> basesBeingResolved)
            {
                return this.Manager.System_Object;
            }

            public override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            public override bool ShouldAddWinRTMembers
            {
                get { return false; }
            }

            public override bool IsWindowsRuntimeImport
            {
                get { return false; }
            }

            public override bool IsComImport
            {
                get { return false; }
            }

            public sealed override ObsoleteAttributeData ObsoleteAttributeData
            {
                get { return null; }
            }

            public override TypeLayout Layout
            {
                get { return default(TypeLayout); }
            }

            public override CharSet MarshallingCharSet
            {
                get { return DefaultMarshallingCharSet; }
            }

            public override bool IsSerializable
            {
                get { return false; }
            }

            public override bool HasDeclarativeSecurity
            {
                get { return false; }
            }

            public override IEnumerable<Microsoft.Cci.SecurityAttribute> GetSecurityInformation()
            {
                throw ExceptionUtilities.Unreachable;
            }

            public override ImmutableArray<string> GetAppliedConditionalSymbols()
            {
                return ImmutableArray<string>.Empty;
            }

            public override AttributeUsageInfo GetAttributeUsageInfo()
            {
                return AttributeUsageInfo.Null;
            }

            public override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
            {
                base.AddSynthesizedAttributes(compilationState, ref attributes);

                AddSynthesizedAttribute(ref attributes, Manager.Compilation.TrySynthesizeAttribute(
                    WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));

                if (Manager.Compilation.Options.OptimizationLevel == OptimizationLevel.Debug)
                {
                    AddSynthesizedAttribute(ref attributes, TrySynthesizeDebuggerDisplayAttribute());
                }
            }

            /// <summary>
            /// Returns a synthesized debugger display attribute or null if one
            /// could not be synthesized.
            /// </summary>
            private SynthesizedAttributeData TrySynthesizeDebuggerDisplayAttribute()
            {
                string displayString;
                if (this.Properties.Length == 0)
                {
                    displayString = "\\{ }";
                }
                else
                {
                    var builder = PooledStringBuilder.GetInstance();
                    var sb = builder.Builder;

                    sb.Append("\\{ ");
                    int displayCount = Math.Min(this.Properties.Length, 10);

                    for (var fieldIndex = 0; fieldIndex < displayCount; fieldIndex++)
                    {
                        string fieldName = this.Properties[fieldIndex].Name;

                        if (fieldIndex > 0)
                        {
                            sb.Append(", ");
                        }

                        sb.Append(fieldName);
                        sb.Append(" = {");
                        sb.Append(fieldName);
                        sb.Append("}");
                    }

                    if (this.Properties.Length > displayCount)
                    {
                        sb.Append(" ...");
                    }

                    sb.Append(" }");
                    displayString = builder.ToStringAndFree();
                }

                return Manager.Compilation.TrySynthesizeAttribute(
                    WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__ctor,
                    arguments: ImmutableArray.Create(new TypedConstant(Manager.System_String, TypedConstantKind.Primitive, displayString)),
                    namedArguments: ImmutableArray.Create(new KeyValuePair<WellKnownMember, TypedConstant>(
                                        WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__Type,
                                        new TypedConstant(Manager.System_String, TypedConstantKind.Primitive, "<Anonymous Type>"))));
            }
        }
    }
}
