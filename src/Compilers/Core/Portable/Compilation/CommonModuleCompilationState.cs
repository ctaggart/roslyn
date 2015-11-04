// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.CodeAnalysis
{
    public class CommonModuleCompilationState
    {
        private bool _frozen;

        public void Freeze()
        {
            Debug.Assert(!_frozen);

            // make sure that values from all threads are visible:
            Interlocked.MemoryBarrier();

            _frozen = true;
        }

        public bool Frozen
        {
            get { return _frozen; }
        }
    }

    public class ModuleCompilationState<TNamedTypeSymbol, TMethodSymbol> : CommonModuleCompilationState
        where TNamedTypeSymbol : class, Cci.INamespaceTypeDefinition
        where TMethodSymbol : class, Cci.IMethodDefinition
    {
        /// <summary>
        /// Maps an async/iterator method to the synthesized state machine type that implements the method. 
        /// </summary>
        private Dictionary<TMethodSymbol, TNamedTypeSymbol> _lazyStateMachineTypes;

        public void SetStateMachineType(TMethodSymbol method, TNamedTypeSymbol stateMachineClass)
        {
            Debug.Assert(!Frozen);

            if (_lazyStateMachineTypes == null)
            {
                Interlocked.CompareExchange(ref _lazyStateMachineTypes, new Dictionary<TMethodSymbol, TNamedTypeSymbol>(), null);
            }

            lock (_lazyStateMachineTypes)
            {
                _lazyStateMachineTypes.Add(method, stateMachineClass);
            }
        }

        public bool TryGetStateMachineType(TMethodSymbol method, out TNamedTypeSymbol stateMachineType)
        {
            Debug.Assert(Frozen);

            stateMachineType = null;
            return _lazyStateMachineTypes != null && _lazyStateMachineTypes.TryGetValue(method, out stateMachineType);
        }
    }
}
