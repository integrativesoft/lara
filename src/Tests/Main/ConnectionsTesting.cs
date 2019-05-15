/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Main;
using System.Net;
using Xunit;

namespace Integrative.Clara.Tests.Main
{
    public class ConnectionsTesting
    {
        [Fact]
        public void ConnectionFound()
        {
            var connections = new Connections();
            var cnx = connections.CreateConnection(IPAddress.Loopback);
            int count = 0;
            foreach (var c in connections.GetConnections())
            {
                count++;
            }
            Assert.Equal(1, count);
            Assert.True(connections.TryGetConnection(cnx.Id, out var found));
            Assert.Same(cnx, found);
        }

        [Fact]
        public void DiscardRemovesConnection()
        {
            var connections = new Connections();
            var cnx = connections.CreateConnection(IPAddress.Loopback);
            connections.Discard(cnx.Id);
            Assert.False(connections.TryGetConnection(cnx.Id, out _));
        }

        [Fact]
        public void DisposeRemovesConnection()
        {
            var connections = new Connections();
            var cnx = connections.CreateConnection(IPAddress.Loopback);
            connections.Dispose();
            Assert.False(connections.TryGetConnection(cnx.Id, out _));
        }
    }
}
