/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    internal static class SemaphoreSlimExtensions
    {
        public static async Task<IDisposable> UseWaitAsync(
            this SemaphoreSlim semaphore,
            CancellationToken cancelToken = default)
        {
            await semaphore.WaitAsync(cancelToken);
            return new ReleaseWrapper(semaphore);
        }

        public static IDisposable UseWait(this SemaphoreSlim semaphore,
            CancellationToken cancelToken = default)
        {
            semaphore.Wait(cancelToken);
            return new ReleaseWrapper(semaphore);
        }

        private class ReleaseWrapper : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;

            private bool _disposed;

            public ReleaseWrapper(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                _semaphore.Release();
            }
        }
    }
}
