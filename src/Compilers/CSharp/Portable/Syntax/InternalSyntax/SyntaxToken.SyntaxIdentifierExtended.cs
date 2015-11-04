// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public partial class SyntaxToken
    {
        public class SyntaxIdentifierExtended : SyntaxIdentifier
        {
            protected readonly SyntaxKind contextualKind;
            protected readonly string valueText;

            public SyntaxIdentifierExtended(SyntaxKind contextualKind, string text, string valueText)
                : base(text)
            {
                this.contextualKind = contextualKind;
                this.valueText = valueText;
            }

            public SyntaxIdentifierExtended(SyntaxKind contextualKind, string text, string valueText, DiagnosticInfo[] diagnostics, SyntaxAnnotation[] annotations)
                : base(text, diagnostics, annotations)
            {
                this.contextualKind = contextualKind;
                this.valueText = valueText;
            }

            public SyntaxIdentifierExtended(ObjectReader reader)
                : base(reader)
            {
                this.contextualKind = (SyntaxKind)reader.ReadInt16();
                this.valueText = reader.ReadString();
            }

            public override Func<ObjectReader, object> GetReader()
            {
                return r => new SyntaxIdentifierExtended(r);
            }

            public override void WriteTo(ObjectWriter writer)
            {
                base.WriteTo(writer);
                writer.WriteInt16((short)this.contextualKind);
                writer.WriteString(this.valueText);
            }

            public override SyntaxKind ContextualKind
            {
                get { return this.contextualKind; }
            }

            public override string ValueText
            {
                get { return this.valueText; }
            }

            public override object Value
            {
                get { return this.valueText; }
            }

            public override SyntaxToken WithLeadingTrivia(CSharpSyntaxNode trivia)
            {
                return new SyntaxIdentifierWithTrivia(this.contextualKind, this.TextField, this.valueText, trivia, null, this.GetDiagnostics(), this.GetAnnotations());
            }

            public override SyntaxToken WithTrailingTrivia(CSharpSyntaxNode trivia)
            {
                return new SyntaxIdentifierWithTrivia(this.contextualKind, this.TextField, this.valueText, null, trivia, this.GetDiagnostics(), this.GetAnnotations());
            }

            public override GreenNode SetDiagnostics(DiagnosticInfo[] diagnostics)
            {
                return new SyntaxIdentifierExtended(this.contextualKind, this.TextField, this.valueText, diagnostics, this.GetAnnotations());
            }

            public override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
            {
                return new SyntaxIdentifierExtended(this.contextualKind, this.TextField, this.valueText, this.GetDiagnostics(), annotations);
            }
        }
    }
}
