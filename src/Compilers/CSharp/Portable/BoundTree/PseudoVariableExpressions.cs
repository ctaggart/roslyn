// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// BoundExpressions to be used for emit. The expressions are assumed
    /// to be lowered and will not be visited by <see cref="BoundTreeWalker"/>.
    /// </summary>
    public abstract class PseudoVariableExpressions
    {
        public abstract BoundExpression GetValue(BoundPseudoVariable variable, DiagnosticBag diagnostics);
        public abstract BoundExpression GetAddress(BoundPseudoVariable variable);
    }
}
