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
        public const int ExpireInterval = 4 * 60;

        private static double _timerInterval = TimerInterval;
        private static int _timerExpires = ExpireInterval;

        public static void SetTimers(double timerInterval, int timerExpires)
        {
            _timerInterval = timerInterval;
            _timerExpires = timerExpires;
        }

        readonly Connections _connections;
        readonly Timer _timer;

        public StaleConnectionsCollector(Connections connections)
        {
            _connections = connections;
            _timer = new Timer
            {
                Interval = _timerInterval
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
                await CleanupNonDisposed();
            }
        }

        private async Task CleanupNonDisposed()
        {
            var minRequired = DateTime.UtcNow.AddMinutes(-_timerExpires);
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

        private async Task CleanupExpired(Connection connection, DateTime minRequired)
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
