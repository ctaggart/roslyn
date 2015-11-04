// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis.Collections;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed partial class SyntaxAndDeclarationManager : CommonSyntaxAndDeclarationManager
    {
        public sealed class State
        {
            public readonly ImmutableArray<SyntaxTree> SyntaxTrees; // In ordinal order.
            public readonly ImmutableDictionary<SyntaxTree, int> OrdinalMap; // Inverse of syntaxTrees array (i.e. maps tree to index)
            public readonly ImmutableDictionary<SyntaxTree, ImmutableArray<LoadDirective>> LoadDirectiveMap;
            public readonly ImmutableDictionary<string, SyntaxTree> LoadedSyntaxTreeMap;
            public readonly ImmutableDictionary<SyntaxTree, Lazy<RootSingleNamespaceDeclaration>> RootNamespaces;
            public readonly DeclarationTable DeclarationTable;

            public State(
                ImmutableArray<SyntaxTree> syntaxTrees,
                ImmutableDictionary<SyntaxTree, int> syntaxTreeOrdinalMap,
                ImmutableDictionary<SyntaxTree, ImmutableArray<LoadDirective>> loadDirectiveMap,
                ImmutableDictionary<string, SyntaxTree> loadedSyntaxTreeMap,
                ImmutableDictionary<SyntaxTree, Lazy<RootSingleNamespaceDeclaration>> rootNamespaces,
                DeclarationTable declarationTable)
            {
                Debug.Assert(syntaxTrees.All(tree => syntaxTrees[syntaxTreeOrdinalMap[tree]] == tree));
                Debug.Assert(syntaxTrees.SetEquals(rootNamespaces.Keys.AsImmutable(), EqualityComparer<SyntaxTree>.Default));

                this.SyntaxTrees = syntaxTrees;
                this.OrdinalMap = syntaxTreeOrdinalMap;
                this.LoadDirectiveMap = loadDirectiveMap;
                this.LoadedSyntaxTreeMap = loadedSyntaxTreeMap;
                this.RootNamespaces = rootNamespaces;
                this.DeclarationTable = declarationTable;
            }
        }
    }
}
