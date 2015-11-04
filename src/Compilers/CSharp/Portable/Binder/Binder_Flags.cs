// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Symbols;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.CSharp
{
    public partial class Binder
    {
        /// <summary>
        /// Represents a small change from the enclosing/next binder.
        /// Can specify a BindingLocation and a ContainingMemberOrLambda.
        /// </summary>
        private sealed class BinderWithContainingMemberOrLambda : Binder
        {
            private readonly Symbol _containingMemberOrLambda;

            public BinderWithContainingMemberOrLambda(Binder next, Symbol containingMemberOrLambda)
                : base(next)
            {
                Debug.Assert(containingMemberOrLambda != null);

                _containingMemberOrLambda = containingMemberOrLambda;
            }

            public BinderWithContainingMemberOrLambda(Binder next, BinderFlags flags, Symbol containingMemberOrLambda)
                : base(next, flags)
            {
                Debug.Assert(containingMemberOrLambda != null);

                _containingMemberOrLambda = containingMemberOrLambda;
            }

            public override Symbol ContainingMemberOrLambda
            {
                get { return _containingMemberOrLambda; }
            }
        }

        /// <summary>
        /// Represents a small change from the enclosing/next binder.
        /// Can specify a receiver Expression for containing conditional member access.
        /// </summary>
        private sealed class BinderWithConditionalReceiver : Binder
        {
            private readonly BoundExpression _receiverExpression;

            public BinderWithConditionalReceiver(Binder next, BoundExpression receiverExpression)
                : base(next)
            {
                Debug.Assert(receiverExpression != null);

                _receiverExpression = receiverExpression;
            }

            public override BoundExpression ConditionalReceiverExpression
            {
                get { return _receiverExpression; }
            }
        }

        public Binder WithFlags(BinderFlags flags)
        {
            return this.Flags == flags
                ? this
                : new Binder(this, flags);
        }

        public Binder WithAdditionalFlags(BinderFlags flags)
        {
            return this.Flags.Includes(flags)
                ? this
                : new Binder(this, this.Flags | flags);
        }

        public Binder WithContainingMemberOrLambda(Symbol containing)
        {
            Debug.Assert((object)containing != null);
            return new BinderWithContainingMemberOrLambda(this, containing);
        }

        /// <remarks>
        /// It seems to be common to do both of these things at once, so provide a way to do so
        /// without adding two links to the binder chain.
        /// </remarks>
        public Binder WithAdditionalFlagsAndContainingMemberOrLambda(BinderFlags flags, Symbol containing)
        {
            Debug.Assert((object)containing != null);
            return new BinderWithContainingMemberOrLambda(this, this.Flags | flags, containing);
        }

        public Binder WithUnsafeRegionIfNecessary(SyntaxTokenList modifiers)
        {
            return (this.Flags.Includes(BinderFlags.UnsafeRegion) || !modifiers.Any(SyntaxKind.UnsafeKeyword))
                ? this
                : new Binder(this, this.Flags | BinderFlags.UnsafeRegion);
        }

        public Binder WithCheckedOrUncheckedRegion(bool @checked)
        {
            Debug.Assert(!this.Flags.Includes(BinderFlags.UncheckedRegion | BinderFlags.CheckedRegion));

            BinderFlags added = @checked ? BinderFlags.CheckedRegion : BinderFlags.UncheckedRegion;
            BinderFlags removed = @checked ? BinderFlags.UncheckedRegion : BinderFlags.CheckedRegion;

            return this.Flags.Includes(added)
                ? this
                : new Binder(this, (this.Flags & ~removed) | added);
        }
    }
}
