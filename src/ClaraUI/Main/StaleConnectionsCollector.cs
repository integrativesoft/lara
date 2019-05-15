/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.DOM;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Integrative.Clara.Main
{
    sealed class StaleConnectionsCollector : IDisposable
    {
        const double TimerInterval = 5 * 60000;
        const int MinutesExpires = 4 * 60;

        readonly Connections _connections;
        readonly Timer _timer;

        public StaleConnectionsCollector(Connections connections)
        {
            _connections = connections;
            _timer = new Timer
            {
                Interval = TimerInterval
            };
            _timer.Elapsed += CleanupExpiredHandler;
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

        internal void CleanupExpiredHandler(object sender, ElapsedEventArgs e)
        {
            if (_disposed)
            {
                return;
            }
            var minRequired = DateTime.UtcNow.AddMinutes(-MinutesExpires);
            var list = new List<KeyValuePair<Guid, Connection>>();
            foreach (var pair in _connections.GetConnections())
            {
                CleanupExpired(pair.Value, minRequired);
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

        private void CleanupExpired(Connection connection, DateTime minRequired)
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
                    connection.Discard(pair.Key);
                }
            }
        }
    }
}
