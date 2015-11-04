// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public partial class SyntaxParser
    {
        protected struct ResetPoint
        {
            public readonly int ResetCount;
            public readonly LexerMode Mode;
            public readonly int Position;
            public readonly CSharpSyntaxNode PrevTokenTrailingTrivia;

            public ResetPoint(int resetCount, LexerMode mode, int position, CSharpSyntaxNode prevTokenTrailingTrivia)
            {
                this.ResetCount = resetCount;
                this.Mode = mode;
                this.Position = position;
                this.PrevTokenTrailingTrivia = prevTokenTrailingTrivia;
            }
        }
    }
}
