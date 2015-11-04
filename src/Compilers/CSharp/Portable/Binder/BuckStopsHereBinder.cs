// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// A binder that knows no symbols and will not delegate further.
    /// </summary>
    public partial class BuckStopsHereBinder : Binder
    {
        public BuckStopsHereBinder(CSharpCompilation compilation)
            : base(compilation)
        {
        }

        public override ConsList<LocalSymbol> ImplicitlyTypedLocalsBeingBound
        {
            get
            {
                return ConsList<LocalSymbol>.Empty;
            }
        }

        public override ImportChain ImportChain
        {
            get
            {
                return null;
            }
        }

        public override Imports GetImports(ConsList<Symbol> basesBeingResolved)
        {
            return Imports.Empty;
        }

        protected override SourceLocalSymbol LookupLocal(SyntaxToken nameToken)
        {
            return null;
        }

        public override bool IsAccessibleHelper(Symbol symbol, TypeSymbol accessThroughType, out bool failedThroughTypeCheck, ref HashSet<DiagnosticInfo> useSiteDiagnostics, ConsList<Symbol> basesBeingResolved)
        {
            failedThroughTypeCheck = false;
            return this.IsSymbolAccessibleConditional(symbol, Compilation.Assembly, ref useSiteDiagnostics);
        }

        public override ConstantFieldsInProgress ConstantFieldsInProgress
        {
            get
            {
                return ConstantFieldsInProgress.Empty;
            }
        }

        public override ConsList<FieldSymbol> FieldsBeingBound
        {
            get
            {
                return ConsList<FieldSymbol>.Empty;
            }
        }

        public override LocalSymbol LocalInProgress
        {
            get
            {
                return null;
            }
        }

        protected override bool IsUnboundTypeAllowed(GenericNameSyntax syntax)
        {
            return false;
        }

        public override bool IsInMethodBody
        {
            get
            {
                return false;
            }
        }

        public override bool IsDirectlyInIterator
        {
            get
            {
                return false;
            }
        }

        public override bool IsIndirectlyInIterator
        {
            get
            {
                return false;
            }
        }

        public override GeneratedLabelSymbol BreakLabel
        {
            get
            {
                return null;
            }
        }

        public override GeneratedLabelSymbol ContinueLabel
        {
            get
            {
                return null;
            }
        }

        public override BoundExpression ConditionalReceiverExpression
        {
            get
            {
                return null;
            }
        }

        // This should only be called in the context of syntactically incorrect programs.  In other
        // contexts statements are surrounded by some enclosing method or lambda.
        public override TypeSymbol GetIteratorElementType(YieldStatementSyntax node, DiagnosticBag diagnostics)
        {
            // There's supposed to be an enclosing method or lambda.
            throw ExceptionUtilities.Unreachable;
        }

        public override Symbol ContainingMemberOrLambda
        {
            get
            {
                return null;
            }
        }

        public override Binder GetBinder(CSharpSyntaxNode node)
        {
            return null;
        }

        public override ImmutableArray<LocalSymbol> GetDeclaredLocalsForScope(CSharpSyntaxNode node)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundSwitchStatement BindSwitchExpressionAndSections(SwitchStatementSyntax node, Binder originalBinder, DiagnosticBag diagnostics)
        {
            // There's supposed to be a SwitchBinder (or other overrider of this method) in the chain.
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundForStatement BindForParts(DiagnosticBag diagnostics, Binder originalBinder)
        {
            // There's supposed to be a ForLoopBinder (or other overrider of this method) in the chain.
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundStatement BindForEachParts(DiagnosticBag diagnostics, Binder originalBinder)
        {
            // There's supposed to be a ForEachLoopBinder (or other overrider of this method) in the chain.
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundWhileStatement BindWhileParts(DiagnosticBag diagnostics, Binder originalBinder)
        {
            // There's supposed to be a WhileBinder (or other overrider of this method) in the chain.
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundDoStatement BindDoParts(DiagnosticBag diagnostics, Binder originalBinder)
        {
            // There's supposed to be a WhileBinder (or other overrider of this method) in the chain.
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundStatement BindUsingStatementParts(DiagnosticBag diagnostics, Binder originalBinder)
        {
            // There's supposed to be a UsingStatementBinder (or other overrider of this method) in the chain.
            throw ExceptionUtilities.Unreachable;
        }

        public override BoundStatement BindLockStatementParts(DiagnosticBag diagnostics, Binder originalBinder)
        {
            // There's supposed to be a LockBinder (or other overrider of this method) in the chain.
            throw ExceptionUtilities.Unreachable;
        }

        public override ImmutableArray<LocalSymbol> Locals
        {
            get
            {
                // There's supposed to be a LocalScopeBinder (or other overrider of this method) in the chain.
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override ImmutableArray<LabelSymbol> Labels
        {
            get
            {
                // There's supposed to be a LocalScopeBinder (or other overrider of this method) in the chain.
                throw ExceptionUtilities.Unreachable;
            }
        }

        public override ImmutableHashSet<Symbol> LockedOrDisposedVariables
        {
            get { return ImmutableHashSet.Create<Symbol>(); }
        }
    }
}
