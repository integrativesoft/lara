/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;

namespace Integrative.Lara.Main
{
    sealed class Connections
    {
        readonly Dictionary<Guid, Connection> _connections;
        readonly StaleConnectionsCollector _collector;

        public Connections()
        {
            _connections = new Dictionary<Guid, Connection>();
            _collector = new StaleConnectionsCollector(this);
        }

        public Connection CreateConnection(IPAddress remoteIp)
        {
            var id = CreateCryptographicallySecureGuid();
            var connection = new Connection(id, remoteIp);
            _connections.Add(id, connection);
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
                _connections.Remove(key);
            }
        }

        public void ClearAll()
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
