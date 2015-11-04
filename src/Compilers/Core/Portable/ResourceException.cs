// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace Microsoft.CodeAnalysis
{
    public sealed class ResourceException : Exception
    {
        public ResourceException(string name, Exception inner = null)
            : base(name, inner)
        {
        }
    }
}
