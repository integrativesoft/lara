/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.Main
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

        [Fact]
        public async void TimerCleansUpDocuments()
        {
            var connections = new Connections();
            var connection = connections.CreateConnection(IPAddress.Loopback);
            connection.CreateDocument(new MyPage(), new LaraOptions());
            var required = DateTime.UtcNow.AddSeconds(10);
            Assert.NotEmpty(connection.GetDocuments());
            await StaleConnectionsCollector.CleanupExpired(connection, required);
            Assert.Empty(connection.GetDocuments());
        }

        [Fact]
        public async void TimerCleansUp()
        {
            StaleConnectionsCollector.SetTimers(200, 0.5);
            var connections = new Connections();
            var cnx = connections.CreateConnection(IPAddress.Loopback);
            cnx.CreateDocument(new MyPage(), new LaraOptions());
            Assert.NotEmpty(connections.GetConnections());
            await Task.Delay(1000);
            Assert.Empty(connections.GetConnections());
        }
    }
}
