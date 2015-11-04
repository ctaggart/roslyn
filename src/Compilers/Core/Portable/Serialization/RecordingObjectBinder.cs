// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace Roslyn.Utilities
{
    /// <summary>
    /// A binder that gathers type/reader mappings during object writing
    /// </summary>
    public abstract class RecordingObjectBinder : ObjectBinder
    {
        public abstract void Record(Type type);
        public abstract void Record(object instance);
    }
}
