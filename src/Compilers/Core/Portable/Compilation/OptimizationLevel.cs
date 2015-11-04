// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Diagnostics;

namespace Microsoft.CodeAnalysis
{
    /// <summary>
    /// Determines the level of optimization of the generated code.
    /// </summary>
    public enum OptimizationLevel
    {
        /// <summary>
        /// Disables all optimizations and instruments the generated code to improve debugging experience.
        /// </summary>
        /// <remarks>
        /// The compiler prefers debuggability over performance. Do not use for code running in a production environment.
        /// <list type="bullet">
        /// <item><description>JIT optimizations are disabled via assembly level attribute (<see cref="DebuggableAttribute"/>).</description></item>
        /// <item><description>Edit and Continue is enabled.</description></item>
        /// <item><description>Slots for local variables are not reused, lifetime of local variables is extended to make the values available during debugging.</description></item>
        /// </list>
        /// <para>
        /// Corresponds to command line argument /optimize-.
        /// </para>
        /// </remarks>
        Debug = 0,

        /// <summary>
        /// Enables all optimizations, debugging experience might be degraded.
        /// </summary>
        /// <remarks>
        /// The compiler prefers performance over debuggability. Use for code running in a production environment.
        /// <list type="bullet">
        /// <item><description>JIT optimizations are enabled via assembly level attribute (<see cref="DebuggableAttribute"/>).</description></item>
        /// <item><description>Edit and Continue is disabled.</description></item>
        /// <item><description>Sequence points may be optimized away. As a result it might not be possible to place or hit a breakpoint.</description></item>
        /// <item><description>User-defined locals might be optimized away. They might not be available while debugging.</description></item>
        /// </list>
        /// <para>
        /// Corresponds to command line argument /optimize+.
        /// </para>
        /// </remarks>
        Release = 1
    }

    public static partial class EnumBounds
    {
        public static bool IsValid(this OptimizationLevel value)
        {
            return value >= OptimizationLevel.Debug && value <= OptimizationLevel.Release;
        }
    }
}
