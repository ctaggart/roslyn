// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;

namespace Microsoft.CodeAnalysis
{
    public partial class CommonReferenceManager<TCompilation, TAssemblySymbol>
    {
        /// <summary>
        /// Result of binding an input assembly and its references. 
        /// </summary>
        [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
        public struct BoundInputAssembly
        {
            /// <summary>
            /// Suitable AssemblySymbol instance for the corresponding assembly, 
            /// null reference if none is available/found.
            /// </summary>
            public TAssemblySymbol AssemblySymbol;

            /// <summary>
            /// For each AssemblyRef of this AssemblyDef specifies which AssemblyDef matches the reference.
            /// </summary>
            /// <remarks>
            /// Result of resolving assembly references of the corresponding assembly 
            /// against provided set of assemblies. Essentially, this is an array returned by
            /// AssemblyData.BindAssemblyReferences method. 
            /// 
            /// Each element describes the assembly the corresponding reference of the input assembly 
            /// is bound to.
            /// </remarks>
            public AssemblyReferenceBinding[] ReferenceBinding;

            private string GetDebuggerDisplay()
            {
                return AssemblySymbol == null ? "?" : AssemblySymbol.ToString();
            }
        }
    }
}
