// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    /// <summary>
    /// Async methods have both a return type (void, Task, or Task&lt;T&gt;) and a 'result' type, which is the
    /// operand type of any return expressions in the async method. The result type is void in the case of
    /// Task-returning and void-returning async methods, and T in the case of Task&lt;T&gt;-returning async
    /// methods.
    /// 
    /// System.Runtime.CompilerServices provides a collection of async method builders that are used in the
    /// generated code of async methods to create and manipulate the async method's task. There are three
    /// distinct async method builder types, one of each async return type: AsyncVoidMethodBuilder,
    /// AsyncTaskMethodBuilder, and AsyncTaskMethodBuilder&lt;T&gt;. 
    /// 
    /// AsyncMethodBuilderMemberCollection provides a common mechanism for accessing the well-known members of
    /// each async method builder type. This avoids having to inspect the return style of the current async method
    /// to pick the right async method builder member during async rewriting.
    /// </summary>
    public struct AsyncMethodBuilderMemberCollection
    {
        /// <summary>
        /// The builder's constructed type.
        /// </summary>
        public readonly NamedTypeSymbol BuilderType;

        /// <summary>
        /// The result type of the constructed task: T for Task&lt;T&gt;, void otherwise.
        /// </summary>
        public readonly TypeSymbol ResultType;

        /// <summary>
        /// Binds an exception to the method builder.
        /// </summary>
        public readonly MethodSymbol SetException;

        /// <summary>
        /// Marks the method builder as successfully completed, and sets the result if method is Task&lt;T&gt;-returning.
        /// </summary>
        public readonly MethodSymbol SetResult;

        /// <summary>
        /// Schedules the state machine to proceed to the next action when the specified awaiter completes.
        /// </summary>
        public readonly MethodSymbol AwaitOnCompleted;

        /// <summary>
        /// Schedules the state machine to proceed to the next action when the specified awaiter completes. This method can be called from partially trusted code.
        /// </summary>
        public readonly MethodSymbol AwaitUnsafeOnCompleted;

        /// <summary>
        /// Begins running the builder with the associated state machine.
        /// </summary>
        public readonly MethodSymbol Start;

        /// <summary>
        /// Associates the builder with the specified state machine.
        /// </summary>
        public readonly MethodSymbol SetStateMachine;

        /// <summary>
        /// Get the constructed task for a Task-returning or Task&lt;T&gt;-returning async method.
        /// </summary>
        public readonly PropertySymbol Task;

        public AsyncMethodBuilderMemberCollection(
            NamedTypeSymbol builderType,
            TypeSymbol resultType,
            MethodSymbol setException,
            MethodSymbol setResult,
            MethodSymbol awaitOnCompleted,
            MethodSymbol awaitUnsafeOnCompleted,
            MethodSymbol start,
            MethodSymbol setStateMachine,
            PropertySymbol task)
        {
            BuilderType = builderType;
            ResultType = resultType;
            SetException = setException;
            SetResult = setResult;
            AwaitOnCompleted = awaitOnCompleted;
            AwaitUnsafeOnCompleted = awaitUnsafeOnCompleted;
            Start = start;
            SetStateMachine = setStateMachine;
            Task = task;
        }

        public static bool TryCreate(SyntheticBoundNodeFactory F, MethodSymbol method, TypeMap typeMap, out AsyncMethodBuilderMemberCollection collection)
        {
            if (method.IsVoidReturningAsync())
            {
                return TryCreate(
                    F: F,

                    builderType: F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncVoidMethodBuilder),
                    resultType: F.SpecialType(SpecialType.System_Void),

                    setException: WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetException,
                    setResult: WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetResult,
                    awaitOnCompleted: WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__AwaitOnCompleted,
                    awaitUnsafeOnCompleted: WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__AwaitUnsafeOnCompleted,
                    start: WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__Start_T,
                    setStateMachine: WellKnownMember.System_Runtime_CompilerServices_AsyncVoidMethodBuilder__SetStateMachine,
                    task: null,
                    collection: out collection);
            }

            if (method.IsTaskReturningAsync(F.Compilation))
            {
                NamedTypeSymbol builderType = F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder);

                PropertySymbol task;
                if (!TryGetWellKnownPropertyAsMember(F, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Task, builderType, out task))
                {
                    collection = default(AsyncMethodBuilderMemberCollection);
                    return false;
                }

                return TryCreate(
                    F: F,

                    builderType: F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder),
                    resultType: F.SpecialType(SpecialType.System_Void),

                    setException: WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetException,
                    setResult: WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetResult,
                    awaitOnCompleted: WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__AwaitOnCompleted,
                    awaitUnsafeOnCompleted: WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__AwaitUnsafeOnCompleted,
                    start: WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__Start_T,
                    setStateMachine: WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder__SetStateMachine,
                    task: task,
                    collection: out collection);
            }

            if (method.IsGenericTaskReturningAsync(F.Compilation))
            {
                TypeSymbol resultType = method.ReturnType.GetMemberTypeArgumentsNoUseSiteDiagnostics().Single();

                if (resultType.IsDynamic())
                {
                    resultType = F.SpecialType(SpecialType.System_Object);
                }

                if (typeMap != null)
                {
                    resultType = typeMap.SubstituteType(resultType).Type;
                }

                NamedTypeSymbol builderType = F.WellKnownType(WellKnownType.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T).Construct(resultType);

                PropertySymbol task;
                if (!TryGetWellKnownPropertyAsMember(F, WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Task, builderType, out task))
                {
                    collection = default(AsyncMethodBuilderMemberCollection);
                    return false;
                }

                return TryCreate(
                    F: F,

                    builderType: builderType,
                    resultType: resultType,

                    setException: WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetException,
                    setResult: WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetResult,
                    awaitOnCompleted: WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__AwaitOnCompleted,
                    awaitUnsafeOnCompleted: WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__AwaitUnsafeOnCompleted,
                    start: WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__Start_T,
                    setStateMachine: WellKnownMember.System_Runtime_CompilerServices_AsyncTaskMethodBuilder_T__SetStateMachine,
                    task: task,
                    collection: out collection);
            }

            throw ExceptionUtilities.UnexpectedValue(method);
        }

        private static bool TryCreate(
            SyntheticBoundNodeFactory F,
            NamedTypeSymbol builderType,
            TypeSymbol resultType,
            WellKnownMember setException,
            WellKnownMember setResult,
            WellKnownMember awaitOnCompleted,
            WellKnownMember awaitUnsafeOnCompleted,
            WellKnownMember start,
            WellKnownMember setStateMachine,
            PropertySymbol task,
            out AsyncMethodBuilderMemberCollection collection)
        {
            MethodSymbol setExceptionMethod;
            MethodSymbol setResultMethod;
            MethodSymbol awaitOnCompletedMethod;
            MethodSymbol awaitUnsafeOnCompletedMethod;
            MethodSymbol startMethod;
            MethodSymbol setStateMachineMethod;

            if (TryGetWellKnownMethodAsMember(F, setException, builderType, out setExceptionMethod) &&
                TryGetWellKnownMethodAsMember(F, setResult, builderType, out setResultMethod) &&
                TryGetWellKnownMethodAsMember(F, awaitOnCompleted, builderType, out awaitOnCompletedMethod) &&
                TryGetWellKnownMethodAsMember(F, awaitUnsafeOnCompleted, builderType, out awaitUnsafeOnCompletedMethod) &&
                TryGetWellKnownMethodAsMember(F, start, builderType, out startMethod) &&
                TryGetWellKnownMethodAsMember(F, setStateMachine, builderType, out setStateMachineMethod))
            {
                collection = new AsyncMethodBuilderMemberCollection(
                    builderType,
                    resultType,
                    setExceptionMethod,
                    setResultMethod,
                    awaitOnCompletedMethod,
                    awaitUnsafeOnCompletedMethod,
                    startMethod,
                    setStateMachineMethod,
                    task);

                return true;
            }

            collection = default(AsyncMethodBuilderMemberCollection);
            return false;
        }

        private static bool TryGetWellKnownMethodAsMember(SyntheticBoundNodeFactory F, WellKnownMember wellKnownMethod, NamedTypeSymbol containingType, out MethodSymbol methodSymbol)
        {
            methodSymbol = F.WellKnownMember(wellKnownMethod) as MethodSymbol;
            if ((object)methodSymbol == null) return false;

            methodSymbol = methodSymbol.AsMember(containingType);
            return true;
        }

        private static bool TryGetWellKnownPropertyAsMember(SyntheticBoundNodeFactory F, WellKnownMember wellKnownProperty, NamedTypeSymbol containingType, out PropertySymbol propertySymbol)
        {
            propertySymbol = F.WellKnownMember(wellKnownProperty) as PropertySymbol;
            if ((object)propertySymbol == null) return false;

            propertySymbol = propertySymbol.AsMember(containingType);
            return true;
        }
    }
}
