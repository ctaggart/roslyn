// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.CodeAnalysis.CSharp
{
    public enum BestIndexKind
    {
        None,
        Best,
        Ambiguous
    }

    public struct BestIndex
    {
        public readonly BestIndexKind Kind;
        public readonly int Best;
        public readonly int Ambiguous1;
        public readonly int Ambiguous2;

        public static BestIndex None() { return new BestIndex(BestIndexKind.None, 0, 0, 0); }
        public static BestIndex HasBest(int best) { return new BestIndex(BestIndexKind.Best, best, 0, 0); }
        public static BestIndex IsAmbiguous(int ambig1, int ambig2) { return new BestIndex(BestIndexKind.Ambiguous, 0, ambig1, ambig2); }

        private BestIndex(BestIndexKind kind, int best, int ambig1, int ambig2)
        {
            this.Kind = kind;
            this.Best = best;
            this.Ambiguous1 = ambig1;
            this.Ambiguous2 = ambig2;
        }
    }
}
