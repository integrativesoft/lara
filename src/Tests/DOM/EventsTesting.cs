/*
Copyright (c) 2019 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tests.Main;
using Integrative.Lara.Tests.Middleware;
using Moq;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.DOM
{
    public class EventsTesting : DummyContextTesting
    {
        [Fact]
        public async void AddRemoveHandler()
        {
            var x = new MessageTypeRegistry();
            var counter = 0;
            Task Handler(MessageEventArgs args1)
            {
                Assert.Equal("test", args1.Body);
                counter++;
                return Task.CompletedTask;
            }
            x.Add(Handler);
            var args = new MessageEventArgs("test");
            await x.RunAll(args);
            Assert.Equal(1, counter);
            x.Remove(Handler);
            await x.RunAll(args);
            Assert.Equal(1, counter);
        }

        [Fact]
        public async void AddRemoveHandlerRegistry()
        {
            CreateMessageContext();

            var document = DomOperationsTesting.CreateDocument();
            var x = new MessageRegistry(document);
            var counter = 0;
            Task Handler(MessageEventArgs args)
            {
                counter++;
                return Task.CompletedTask;
            }
            x.Add("a", Handler);
            await document.Head.NotifyEvent("_a");
            Assert.Equal(1, counter);

            x.Remove("b", Handler);
            await document.Head.NotifyEvent("_a");
            Assert.Equal(2, counter);

            x.Remove("a", Handler);
            await document.Head.NotifyEvent("_a");
            Assert.Equal(2, counter);
        }

        private void CreateMessageContext()
        {
            var context = new Mock<IPageContext>();
            LaraUI.InternalContext.Value = context.Object;
            var bridge = new Mock<IJSBridge>();
            context.Setup(x => x.JSBridge).Returns(bridge.Object);
            bridge.Setup(x => x.EventData).Returns("test");
            context.Setup(x => x.Application).Returns(Context.Application);
        }

        [Fact]
        public void DebounceStored()
        {
            var x = new EventSettings
            {
                DebounceInterval = 5
            };
            Assert.Equal(5, x.DebounceInterval);
        }

        [Fact]
        public void DocumentProcessesMessageListeners()
        {
            CreateMessageContext();
            var doc = DomOperationsTesting.CreateDocument();
            var counter = 0;
            Task Handler(MessageEventArgs args)
            {
                counter++;
                return Task.CompletedTask;
            }
            doc.AddMessageListener("a", Handler);
            doc.Head.NotifyEvent("_a");
            Assert.Equal(1, counter);
            doc.RemoveMessageListener("a", Handler);
            doc.Head.NotifyEvent("_a");
            Assert.Equal(1, counter);
        }

        [Fact]
        public void DocumentOnMessageRuns()
        {
            CreateMessageContext();
            var doc = DomOperationsTesting.CreateDocument();
            var counter = 0;
            Task Handler()
            {
                counter++;
                return Task.CompletedTask;
            }
            doc.OnMessage("a", Handler);
            doc.Head.NotifyEvent("_a");
            Assert.Equal(1, counter);
        }

        [Fact]
        public async void AsyncEventDispatches()
        {
            var counter = 0;
            var ev = new AsyncEvent();
            Task MyMethod()
            {
                counter++;
                return Task.CompletedTask;
            }
            ev.Subscribe(MyMethod);
            await ev.InvokeAsync(this, new EventArgs());
            Assert.Equal(1, counter);
            
            ev.Unsubscribe(MyMethod);
            await ev.InvokeAsync(this, new EventArgs());
            Assert.Equal(1, counter);
        }

        [Fact]
        public async void AsyncEventPassesAlong()
        {
            int counter = 0;
            var ev = new AsyncEvent();
            ev.Subscribe(() =>
            {
                counter++;
                return Task.CompletedTask;
            });
            var ev2 = new AsyncEvent();
            ev2.Subscribe(ev);
            await ev2.InvokeAsync(this, new EventArgs());
            Assert.Equal(1, counter);

            ev2.Unsubscribe(ev);
            await ev2.InvokeAsync(this, new EventArgs());
            Assert.Equal(1, counter);
        }

        [Fact]
        public async void AsyncEventSubscribeHandler()
        {
            var counter = 0;
            Task MyMethod(object sender, EventArgs args)
            {
                counter++;
                return Task.CompletedTask;
            }
            var handler = new AsyncEventHandler<EventArgs>(MyMethod);
            var ev = new AsyncEvent();
            ev.Subscribe(handler);
            await ev.InvokeAsync(this, new EventArgs());
            Assert.Equal(1, counter);

            ev.Unsubscribe(handler);
            await ev.InvokeAsync(this, new EventArgs());
            Assert.Equal(1, counter);
        }

        [Fact]
        public async void AsyncEventSyncHandler()
        {
            var counter = 0;
            void MyMethod() => counter++;
            var ev = new AsyncEvent<EventArgs>();
            ev.Subscribe(MyMethod);
            await ev.InvokeAsync(this, new EventArgs());
            Assert.Equal(1, counter);

            ev.Unsubscribe(MyMethod);
            await ev.InvokeAsync(this, new EventArgs());
            Assert.Equal(1, counter);
        }

        [Fact]
        public void DocumentEvent()
        {
            int counter = 0;
            var x = new Document(new MyPage(), 100);
            x.On("keyup", () =>
            {
                counter++;
                return Task.CompletedTask;
            });
            x.NotifyEvent("keyup");
            Assert.Equal(1, counter);
            x.NotifyEvent("keyup");
            Assert.Equal(2, counter);
            x.On("keyup", null);
            x.NotifyEvent("keyup");
            Assert.Equal(2, counter);
        }

        [Fact]
        public void DocumentGuidToString()
        {
            var x = new Document(new MyPage(), 100);
            var text = x.VirtualId.ToString(GlobalConstants.GuidFormat, CultureInfo.InvariantCulture);
            Assert.Equal(text, x.VirtualIdString);
        }
    }
}
