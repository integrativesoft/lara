/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tests.Middleware;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.Main
{
    public class ConnectionsTesting : DummyContextTesting
    {
        [Fact]
        public void ConnectionFound()
        {
            using var connections = new Connections();
            var cnx = connections.CreateConnection(IPAddress.Loopback);
            var count = 0;
            foreach (var unused in connections.GetConnections())
            {
                count++;
            }
            Assert.Equal(1, count);
            Assert.True(connections.TryGetConnection(cnx.Id, out var found));
            Assert.Same(cnx, found);
        }

        [Fact]
        public async void DiscardRemovesConnection()
        {
            using var connections = new Connections();
            var cnx = connections.CreateConnection(IPAddress.Loopback);
            await connections.Discard(cnx.Id);
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
            using var connections = new Connections();
            var connection = connections.CreateConnection(IPAddress.Loopback);
            connection.CreateDocument(new MyPage(), BaseModeController.DefaultKeepAliveInterval);
            var required = DateTime.UtcNow.AddSeconds(10);
            Assert.NotEmpty(connection.GetDocuments());
            await StaleConnectionsCollector.CleanupExpired(connection, required);
            Assert.Empty(connection.GetDocuments());
        }

        [Fact]
        public async void TimerCleansUp()
        {
            using var connections = new Connections(200, 100);
            var cnx = connections.CreateConnection(IPAddress.Loopback);
            var document = cnx.CreateDocument(new MyPage(), BaseModeController.DefaultKeepAliveInterval);
            Assert.NotEmpty(connections.GetConnections());
            document.ModifyLastUtcForTesting(DateTime.UtcNow.AddDays(-1));
            await Task.Delay(400);
            Assert.Empty(connections.GetConnections());
        }
    }
}
