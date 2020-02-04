/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tests.Main;
using Moq;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.Middleware
{
    public class ServerEventsTesting : DummyContextTesting
    {
        private readonly Document _document;
        private readonly ServerEventsController _controller;
        private readonly Mock<WebSocket> _socket;

        public ServerEventsTesting()
        {
            var page = new MyPage();
            _document = new Document(page, BaseModeController.DefaultKeepAliveInterval);
            _controller = _document.GetServerEventsController();
            _controller.ServerEventsOn();
            _socket = new Mock<WebSocket>();
        }

        private async Task Initialize()
        {
            await _controller.GetSocketCompletion(_socket.Object);
        }

        [Fact]
        public async void DiscardSocketDiscards()
        {
            await Initialize();
            await _controller.ServerEventsOff();
            Assert.Equal(ServerEventsStatus.Disabled, _controller.ServerEventsStatus);
        }

        [Fact]
        public async void ServerEventRemainsOpen()
        {
            await Initialize();
            Assert.True(_controller.SocketRemainsOpen(GlobalConstants.ServerSideEvent));
        }

        [Fact]
        public async void ServerEventFlushFlushes()
        {
            await Initialize();
            _document.OpenEventQueue();
            _document.Body.AppendText("hello");
            Assert.NotEmpty(_document.GetQueue());
            await _controller.ServerEventFlush();
            Assert.Empty(_document.GetQueue());
        }

        [Fact]
        public async void FlushWhenDisabledRejected()
        {
            await Initialize();
            await _document.ServerEventsOff();
            _document.OpenEventQueue();
            _document.Body.AppendText("hi");
            await _controller.ServerEventsOff();
            var found = false;
            try
            {
                await _controller.ServerEventFlush();
            }
            catch (InvalidOperationException)
            {
                found = true;
            }
            Assert.True(found);
        }

        [Fact]
        public async void NoFlushWhenQueueEmpty()
        {
            await Initialize();
            _document.OpenEventQueue();
            await _controller.ServerEventFlush();
            _socket.Verify(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(),
                It.IsAny<WebSocketMessageType>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async void NoFlushWhenWaitingConnection()
        {
            await Initialize();
            await _controller.ServerEventsOff();
            _controller.ServerEventsOn();
            _document.OpenEventQueue();
            _document.Body.AppendText("hi");
            _controller.ServerEventsOn();
            await _controller.ServerEventFlush();
            _socket.Verify(x => x.SendAsync(It.IsAny<ArraySegment<byte>>(),
                It.IsAny<WebSocketMessageType>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async void AcceptingSocketFlushesPending()
        {
            _document.OpenEventQueue();
            _document.Body.AppendText("hello");
            _controller.ServerEventsOn();
            await _controller.ServerEventFlush();
            Assert.NotEmpty(_document.GetQueue());
            await _document.GetSocketCompletion(_socket.Object);
            Assert.Empty(_document.GetQueue());
        }

        [Fact]
        public async void ServerEventFlushes()
        {
            _document.OpenEventQueue();
            _controller.ServerEventsOn();
            await _controller.GetSocketCompletion(_socket.Object);
            using (_document.StartServerEvent())
            {
                _document.Body.AppendText("hello");
                Assert.NotEmpty(_document.GetQueue());
            }
            Assert.Empty(_document.GetQueue());
        }

        [Fact]
        public async void ServerEventsFlushesPartialChanges()
        {
            _document.OpenEventQueue();
            _controller.ServerEventsOn();
            await _controller.GetSocketCompletion(_socket.Object);
            using var access = _document.StartServerEvent();
            _document.Body.AppendText("hello");
            Assert.NotEmpty(_document.GetQueue());
            await access.FlushPartialChanges();
            Assert.Empty(_document.GetQueue());
        }

        [Fact]
        public async void ServerEventFailDisposed()
        {
            _document.OpenEventQueue();
            _controller.ServerEventsOn();
            await _controller.GetSocketCompletion(_socket.Object);
            var access = _document.StartServerEvent();
            access.Dispose();
            var found = false;
            try
            {
                await access.FlushPartialChanges();
            }
            catch (InvalidOperationException)
            {
                found = true;
            }
            Assert.True(found);
        }
    }
}
