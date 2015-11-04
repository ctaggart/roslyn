// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public partial class SyntaxToken
    {
        public class SyntaxIdentifierWithTrailingTrivia : SyntaxIdentifier
        {
            private readonly CSharpSyntaxNode _trailing;

            public SyntaxIdentifierWithTrailingTrivia(string text, CSharpSyntaxNode trailing)
                : base(text)
            {
                if (trailing != null)
                {
                    this.AdjustFlagsAndWidth(trailing);
                    _trailing = trailing;
                }
            }

            public SyntaxIdentifierWithTrailingTrivia(string text, CSharpSyntaxNode trailing, DiagnosticInfo[] diagnostics, SyntaxAnnotation[] annotations)
                : base(text, diagnostics, annotations)
            {
                if (trailing != null)
                {
                    this.AdjustFlagsAndWidth(trailing);
                    _trailing = trailing;
                }
            }

            public SyntaxIdentifierWithTrailingTrivia(ObjectReader reader)
                : base(reader)
            {
                var trailing = (CSharpSyntaxNode)reader.ReadValue();
                if (trailing != null)
                {
                    this.AdjustFlagsAndWidth(trailing);
                    _trailing = trailing;
                }
            }

            public override Func<ObjectReader, object> GetReader()
            {
                return r => new SyntaxIdentifierWithTrailingTrivia(r);
            }

            public override void WriteTo(ObjectWriter writer)
            {
                base.WriteTo(writer);
                writer.WriteValue(_trailing);
            }

            public override CSharpSyntaxNode GetTrailingTrivia()
            {
                return _trailing;
            }

            public override SyntaxToken WithLeadingTrivia(CSharpSyntaxNode trivia)
            {
                return new SyntaxIdentifierWithTrivia(this.Kind, this.TextField, this.TextField, trivia, _trailing, this.GetDiagnostics(), this.GetAnnotations());
            }

            public override SyntaxToken WithTrailingTrivia(CSharpSyntaxNode trivia)
            {
                return new SyntaxIdentifierWithTrailingTrivia(this.TextField, trivia, this.GetDiagnostics(), this.GetAnnotations());
            }

            public override GreenNode SetDiagnostics(DiagnosticInfo[] diagnostics)
            {
                return new SyntaxIdentifierWithTrailingTrivia(this.TextField, _trailing, diagnostics, this.GetAnnotations());
            }

            public override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
            {
                return new SyntaxIdentifierWithTrailingTrivia(this.TextField, _trailing, this.GetDiagnostics(), annotations);
            }
        }
    }
}
