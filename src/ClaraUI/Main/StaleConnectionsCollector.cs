/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Timers;

namespace Integrative.Clara.Main
{
    sealed class StaleConnectionsCollector : IDisposable
    {
        const double TimerInterval = 5 * 60000;
        const int HoursExpires = 4;

        readonly Connections _connections;
        readonly Timer _timer;

        public StaleConnectionsCollector(Connections connections)
        {
            _connections = connections;
            _timer = new Timer
            {
                Interval = TimerInterval
            };
            _timer.Elapsed += CleanupExpired;
            _timer.Start();
        }

        bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _timer.Stop();
            }
        }

        private void CleanupExpired(object sender, ElapsedEventArgs e)
        {
            if (_disposed)
            {
                return;
            }
            var minRequired = DateTime.UtcNow.AddHours(-HoursExpires);
            foreach (var pair in _connections.GetConnections())
            {
                CleanupExpired(pair.Value, minRequired);
                if (pair.Value.IsEmpty)
                {
                    _connections.Discard(pair.Key);
                }
            }
        }

        private void CleanupExpired(Connection connection, DateTime minRequired)
        {
            foreach (var pair in connection.GetDocuments())
            {
                var utc = pair.Value.LastUTC;
                if (utc < minRequired)
                {
                    connection.Discard(pair.Key);
                }
            }
        }
    }
}
