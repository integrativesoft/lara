/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Main;
using System;
using System.Net;
using Xunit;

namespace Integrative.Clara.Tests.Main
{
    public class StaleTesting
    {
        [Fact]
        public void CleanupLeavesUnexpiredDocument()
        {
            var connections = new Connections();
            var cnx = connections.CreateConnection(IPAddress.Loopback);
            var doc1 = cnx.CreateDocument(new MyPage());
            var doc2 = cnx.CreateDocument(new MyPage());
            doc2.ModifyLastUtcForTesting(DateTime.UtcNow.AddHours(-10));
            using (var collector = new StaleConnectionsCollector(connections))
            {
                collector.CleanupExpiredHandler(this, null);
            }
            Assert.True(cnx.TryGetDocument(doc1.VirtualId, out _));
            Assert.False(cnx.TryGetDocument(doc2.VirtualId, out _));
        }

        [Fact]
        public void EmptyConnectionGetsCollected()
        {
            var connections = new Connections();
            var cnx = connections.CreateConnection(IPAddress.Loopback);
            using (var collector = new StaleConnectionsCollector(connections))
            {
                collector.CleanupExpiredHandler(this, null);
            }
            Assert.False(connections.TryGetConnection(cnx.Id, out _));
        }

        [Fact]
        public void DisposedCollectorWontExecute()
        {
            var connections = new Connections();
            var cnx = connections.CreateConnection(IPAddress.Loopback);
            var collector = new StaleConnectionsCollector(connections);
            collector.Dispose();
            collector.CleanupExpiredHandler(this, null);
            Assert.True(connections.TryGetConnection(cnx.Id, out _));
        }
    }
}
