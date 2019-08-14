/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace Integrative.Lara.Main
{
    sealed class StaleConnectionsCollector : IDisposable
    {
        public const double TimerInterval = 5 * 60000;
        public const double ExpireInterval = 4 * 3600 * 1000;

        readonly Connections _connections;
        readonly Timer _timer;
        readonly double _expireInterval;

        public StaleConnectionsCollector(Connections connections)
            : this(connections, TimerInterval, ExpireInterval)
        {
        }

        public StaleConnectionsCollector(Connections connections, double timerInterval, double expireInternal)
        {
            _connections = connections;
            _expireInterval = expireInternal;
            _timer = new Timer
            {
                Interval = timerInterval
            };
            _timer.Elapsed += async (sender, args) => await CleanupExpiredHandler();
            _timer.Start();
        }

        bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _timer.Stop();
                _timer.Dispose();
            }
        }

        internal async Task CleanupExpiredHandler()
        {
            if (!_disposed)
            {
                _timer.Enabled = false;
                await CleanupNonDisposed();
                _timer.Enabled = true;
            }
        }

        private async Task CleanupNonDisposed()
        {
            var minRequired = DateTime.UtcNow.AddMilliseconds(-_expireInterval);
            var list = new List<KeyValuePair<Guid, Connection>>();
            foreach (var pair in _connections.GetConnections())
            {
                await CleanupExpired(pair.Value, minRequired);
                if (pair.Value.IsEmpty)
                {
                    list.Add(pair);
                }
            }
            foreach (var pair in list)
            {
                if (pair.Value.IsEmpty)
                {
                    _connections.Discard(pair.Key);
                }
            }
        }

        internal static async Task CleanupExpired(Connection connection, DateTime minRequired)
        {
            var list = new List<KeyValuePair<Guid, Document>>();
            foreach (var pair in connection.GetDocuments())
            {
                if (pair.Value.LastUTC < minRequired)
                {
                    list.Add(pair);
                }
            }
            foreach (var pair in list)
            {
                if (pair.Value.LastUTC < minRequired)
                {
                    await connection.Discard(pair.Key);
                }
            }
        }
    }
}
