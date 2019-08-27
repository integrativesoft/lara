/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;

namespace Integrative.Lara.Main
{
    sealed class Connections
    {
        readonly ConcurrentDictionary<Guid, Connection> _connections;
        readonly StaleConnectionsCollector _collector;

        public Connections()
        {
            _connections = new ConcurrentDictionary<Guid, Connection>();
            _collector = new StaleConnectionsCollector(this);
        }

        public Connections(double cleanupInterval, double expireInterval)
        {
            _connections = new ConcurrentDictionary<Guid, Connection>();
            _collector = new StaleConnectionsCollector(this, cleanupInterval, expireInterval);
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

        public void Discard(Guid key)
        {
            if (_connections.TryGetValue(key, out var connection))
            {
                connection.Close();
                _connections.TryRemove(key, out _);
            }
        }

        public void Clear()
        {
            _connections.Clear();
        }

        public static Guid CreateCryptographicallySecureGuid()
        {
            using (var provider = new RNGCryptoServiceProvider())
            {
                var bytes = new byte[16];
                provider.GetBytes(bytes);
                return new Guid(bytes);
            }
        }

        public void ClearEmptyConnection(Connection connection)
        {
            if (connection.IsEmpty)
            {
                Discard(connection.Id);
            }
        }

        public IEnumerable<KeyValuePair<Guid, Connection>> GetConnections() => _connections;

        bool _disposed;

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
