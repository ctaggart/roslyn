// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public partial class SyntaxList
    {
        public class WithTwoChildren : SyntaxList
        {
            private readonly CSharpSyntaxNode _child0;
            private readonly CSharpSyntaxNode _child1;

            public WithTwoChildren(CSharpSyntaxNode child0, CSharpSyntaxNode child1)
            {
                this.SlotCount = 2;
                this.AdjustFlagsAndWidth(child0);
                _child0 = child0;
                this.AdjustFlagsAndWidth(child1);
                _child1 = child1;
            }

            public WithTwoChildren(ObjectReader reader)
                : base(reader)
            {
                this.SlotCount = 2;
                _child0 = (CSharpSyntaxNode)reader.ReadValue();
                this.AdjustFlagsAndWidth(_child0);
                _child1 = (CSharpSyntaxNode)reader.ReadValue();
                this.AdjustFlagsAndWidth(_child1);
            }

            public override void WriteTo(ObjectWriter writer)
            {
                base.WriteTo(writer);
                writer.WriteValue(_child0);
                writer.WriteValue(_child1);
            }

            public override Func<ObjectReader, object> GetReader()
            {
                return r => new WithTwoChildren(r);
            }

            public override GreenNode GetSlot(int index)
            {
                switch (index)
                {
                    case 0:
                        return _child0;
                    case 1:
                        return _child1;
                    default:
                        return null;
                }
            }

            public override void CopyTo(ArrayElement<CSharpSyntaxNode>[] array, int offset)
            {
                array[offset].Value = _child0;
                array[offset + 1].Value = _child1;
            }

            public override SyntaxNode CreateRed(SyntaxNode parent, int position)
            {
                return new CSharp.Syntax.SyntaxList.WithTwoChildren(this, parent, position);
            }

            public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
            {
                throw new NotImplementedException();
            }

            public override void Accept(CSharpSyntaxVisitor visitor)
            {
                throw new NotImplementedException();
            }
        }
    }
}
