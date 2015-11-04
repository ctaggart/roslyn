// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// Represents implicit, script and submission classes.
    /// </summary>
    public sealed class ImplicitNamedTypeSymbol : SourceMemberContainerTypeSymbol
    {
        public ImplicitNamedTypeSymbol(NamespaceOrTypeSymbol containingSymbol, MergedTypeDeclaration declaration, DiagnosticBag diagnostics)
            : base(containingSymbol, declaration, diagnostics)
        {
            Debug.Assert(declaration.Kind == DeclarationKind.ImplicitClass ||
                         declaration.Kind == DeclarationKind.Submission ||
                         declaration.Kind == DeclarationKind.Script);

            state.NotePartComplete(CompletionPart.EnumUnderlyingType); // No work to do for this.
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            state.NotePartComplete(CompletionPart.Attributes);
            return ImmutableArray<CSharpAttributeData>.Empty;
        }

        public override AttributeUsageInfo GetAttributeUsageInfo()
        {
            return AttributeUsageInfo.Null;
        }

        protected override Location GetCorrespondingBaseListLocation(NamedTypeSymbol @base)
        {
            // A script class may implement interfaces in hosted scenarios.
            // The interface definitions are specified via API, not in compilation source.
            return NoLocation.Singleton;
        }

        public override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics
        {
            get
            {
                return (this.TypeKind == TypeKind.Submission) ? null : this.DeclaringCompilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object);
            }
        }

        protected override void CheckBase(DiagnosticBag diagnostics)
        {
            // check that System.Object is available. 
            // Although submission semantically doesn't have a base class we need to emit one.
            var info = this.DeclaringCompilation.GetSpecialType(SpecialType.System_Object).GetUseSiteDiagnostic();
            if (info != null)
            {
                Symbol.ReportUseSiteDiagnostic(info, diagnostics, Locations[0]);
            }
        }

        public override NamedTypeSymbol GetDeclaredBaseType(ConsList<Symbol> basesBeingResolved)
        {
            return BaseTypeNoUseSiteDiagnostics;
        }

        public override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<Symbol> basesBeingResolved)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<Symbol> basesBeingResolved)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        protected override void CheckInterfaces(DiagnosticBag diagnostics)
        {
            // nop
        }

        public override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get { return ImmutableArray<TypeParameterSymbol>.Empty; }
        }

        public override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics
        {
            get { return ImmutableArray<TypeSymbol>.Empty; }
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
                return ImmutableArray<ImmutableArray<CustomModifier>>.Empty;
            }
        }

        public override bool IsComImport
        {
            get { return false; }
        }

        public override NamedTypeSymbol ComImportCoClass
        {
            get { return null; }
        }

        public override bool HasSpecialName
        {
            get { return false; }
        }

        public override bool ShouldAddWinRTMembers
        {
            get { return false; }
        }

        public sealed override bool IsWindowsRuntimeImport
        {
            get { return false; }
        }

        public sealed override bool IsSerializable
        {
            get { return false; }
        }

        public sealed override TypeLayout Layout
        {
            get { return default(TypeLayout); }
        }

        public bool HasStructLayoutAttribute
        {
            get { return false; }
        }

        public override CharSet MarshallingCharSet
        {
            get { return DefaultMarshallingCharSet; }
        }

        public sealed override bool HasDeclarativeSecurity
        {
            get { return false; }
        }

        public sealed override IEnumerable<Microsoft.Cci.SecurityAttribute> GetSecurityInformation()
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        public override ObsoleteAttributeData ObsoleteAttributeData
        {
            get { return null; }
        }
    }
}
