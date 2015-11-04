// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Threading;

namespace Roslyn.Utilities
{
    public static class ReaderWriterLockSlimExtensions
    {
        public static ReadLockExiter DisposableRead(this ReaderWriterLockSlim @lock)
        {
            return new ReadLockExiter(@lock);
        }

        public struct ReadLockExiter : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;

            public ReadLockExiter(ReaderWriterLockSlim @lock)
            {
                _lock = @lock;
                @lock.EnterReadLock();
            }

            public void Dispose()
            {
                _lock.ExitReadLock();
            }
        }

        public static WriteLockExiter DisposableWrite(this ReaderWriterLockSlim @lock)
        {
            return new WriteLockExiter(@lock);
        }

        public struct WriteLockExiter : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;

            public WriteLockExiter(ReaderWriterLockSlim @lock)
            {
                _lock = @lock;
                @lock.EnterWriteLock();
            }

            public void Dispose()
            {
                _lock.ExitWriteLock();
            }
        }

        public static void AssertCanRead(this ReaderWriterLockSlim @lock)
        {
            if (!@lock.IsReadLockHeld && !@lock.IsUpgradeableReadLockHeld && !@lock.IsWriteLockHeld)
            {
                throw new InvalidOperationException();
            }
        }

        public static void AssertCanWrite(this ReaderWriterLockSlim @lock)
        {
            if (!@lock.IsWriteLockHeld)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
