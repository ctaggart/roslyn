// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.CodeGen
{
    public partial class ILBuilder
    {
        /// <summary>
        /// Contains information about a label.
        /// </summary>
        private struct LabelInfo
        {
            //some labels can be jumped to only with non zero stack depth.
            //all jumps must agree on the stack depth as well as if we reach the 
            //label via fall through.
            //if a label is marked before any branches to the label have been seen
            //the stack is considered to be 0.
            public readonly int stack;
            public readonly BasicBlock bb;

            /// <summary>
            /// Sometimes we need to know if a label is targeted by conditional branches.
            /// For example optimizer can do optimizations of branches into outer try scopes only 
            /// if they are unconditional (because there are no conditional Leave opcodes)
            /// </summary>
            public readonly bool targetOfConditionalBranches;

            /// <summary>
            /// Used when we see a branch, but label is not yet marked.
            /// </summary>
            public LabelInfo(int stack, bool targetOfConditionalBranches)
                : this(null, stack, targetOfConditionalBranches)
            {
            }

            /// <summary>
            /// Used when label is marked to the code.
            /// </summary>
            public LabelInfo(BasicBlock bb, int stack, bool targetOfConditionalBranches)
            {
                this.stack = stack;
                this.bb = bb;
                this.targetOfConditionalBranches = targetOfConditionalBranches;
            }

            public LabelInfo WithNewTarget(BasicBlock bb)
            {
                return new LabelInfo(bb, this.stack, this.targetOfConditionalBranches);
            }

            public LabelInfo SetTargetOfConditionalBranches()
            {
                return new LabelInfo(this.bb, this.stack, true);
            }
        }
    }
}
