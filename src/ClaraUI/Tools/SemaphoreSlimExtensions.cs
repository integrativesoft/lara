/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Integrative.Clara.Tools
{
    public static class SemaphoreSlimExtensions
    {
        public static async Task<IDisposable> UseWaitAsync(
            this SemaphoreSlim semaphore,
            CancellationToken cancelToken = default)
        {
            await semaphore.WaitAsync(cancelToken).ConfigureAwait(false);
            return new ReleaseWrapper(semaphore);
        }

        private class ReleaseWrapper : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;

            private bool _isDisposed;

            public ReleaseWrapper(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                if (_isDisposed)
                {
                    return;
                }
                _semaphore.Release();
                _isDisposed = true;
            }
        }
    }
}
