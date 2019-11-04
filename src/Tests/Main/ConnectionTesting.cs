/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Integrative.Lara.Middleware;
using Integrative.Lara.Tests.Middleware;
using System.Net;
using Xunit;

namespace Integrative.Lara.Tests.Main
{
    public class ConnectionTesting : DummyContextTesting
    {
        [Fact]
        public void NonExistingDocument()
        {
            var guid = Connections.CreateCryptographicallySecureGuid();
            var connection = new Connection(guid, IPAddress.Loopback);
            Assert.True(IPAddress.Loopback.Equals(connection.RemoteIP));
            Assert.True(connection.IsEmpty);
            Assert.False(connection.TryGetDocument(guid, out _));
        }

        [Fact]
        public void CreateDocumentPresent()
        {
            var connectionId = Connections.CreateCryptographicallySecureGuid();
            var connection = new Connection(connectionId, IPAddress.Loopback);
            var document = connection.CreateDocument(new MyPage(), BaseModeController.DefaultKeepAliveInterval);
            int count = 0;
            foreach (var pair in connection.GetDocuments())
            {
                Assert.Same(document, pair.Value);
                count++;
            }
            Assert.Equal(1, count);
            Assert.True(connection.TryGetDocument(document.VirtualId, out var found));
            Assert.Same(document, found);
            Assert.False(connection.IsEmpty);
        }

        [Fact]
        public async void DiscardRemovesDocument()
        {
            var connectionId = Connections.CreateCryptographicallySecureGuid();
            var connection = new Connection(connectionId, IPAddress.Loopback);
            var page = new MyPage();
            var document = connection.CreateDocument(page, BaseModeController.DefaultKeepAliveInterval);
            await connection.Discard(document.VirtualId);
            Assert.False(connection.TryGetDocument(document.VirtualId, out _));
            Assert.True(page.Disposed);
        }
    }
}
