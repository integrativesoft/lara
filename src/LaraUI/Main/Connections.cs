/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    internal sealed class Connections : IDisposable
    {
        private readonly ConcurrentDictionary<Guid, Connection> _connections;
        private readonly StaleConnectionsCollector _collector;

        public Connections()
            : this(StaleConnectionsCollector.DefaultTimerInterval, StaleConnectionsCollector.DefaultExpireInterval)
        {
        }

        public Connections(double cleanupInterval, double expireInterval)
        {
            _connections = new ConcurrentDictionary<Guid, Connection>();
            _collector = new StaleConnectionsCollector(this, cleanupInterval, expireInterval);
        }

        public double StaleCollectionInterval
        {
            get => _collector.TimerInterval;
            set => _collector.TimerInterval = value;
        }

        public double StaleExpirationInterval
        {
            get => _collector.ExpirationInterval;
            set => _collector.ExpirationInterval = value;
        }

        public Connection CreateConnection(IPAddress remoteIp)
        {
            var id = CreateCryptographicallySecureGuid();
            var connection = new Connection(id, remoteIp);
            _connections.TryAdd(id, connection);
            return connection;
        }

        public bool TryGetConnection(Guid id, out Connection connection)
        {
            return _connections.TryGetValue(id, out connection);
        }

        public async Task Discard(Guid key)
        {
            if (_connections.TryGetValue(key, out var connection))
            {
                await connection.Close();
                _connections.TryRemove(key, out _);
            }
        }

        public void Clear()
        {
            _connections.Clear();
        }

        public static Guid CreateCryptographicallySecureGuid()
        {
            using var provider = new RNGCryptoServiceProvider();
            var bytes = new byte[16];
            provider.GetBytes(bytes);
            return new Guid(bytes);
        }

        public async Task ClearEmptyConnection(Connection connection)
        {
            if (connection.IsEmpty)
            {
                await Discard(connection.Id);
            }
        }

        public IEnumerable<KeyValuePair<Guid, Connection>> GetConnections() => _connections;

        private bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _connections.Clear();
                _collector.Dispose();
            }
        }
    }
}
