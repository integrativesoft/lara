/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// The ServerEvent disposable class represents the life cycle of a server-side event.
    /// </summary>
    public sealed class ServerEvent : IDisposable
    {
        private readonly Document _document;
        private readonly IDisposable _access;

        private bool _disposed;

        internal ServerEvent(Document document)
        {
            _document = document;
            _access = document.Semaphore.UseWait();
        }

        /// <summary>
        /// Flushes partial changes made to the document. 
        /// </summary>
        /// <returns>Task</returns>
        public Task FlushPartialChanges()
        {
            VerifyNotDisposed();
            return _document.ServerEventFlush();
        }

        internal void VerifyNotDisposed()
        {
            if (_disposed)
            {
                throw new InvalidOperationException(Resources.ServerEventAlreadyDisposed);
            }
        }

        /// <summary>
        /// The dispose method flushes all pending changes in the document.
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            _document.ServerEventFlush().Wait();
            _access.Dispose();
        }
    }
}
