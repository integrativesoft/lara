/*
Copyright (c) 2019 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
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
            int counter = 0;
            Task handler(MessageEventArgs args)
            {
                Assert.Equal("test", args.Body);
                counter++;
                return Task.CompletedTask;
            }
            x.Add(handler);
            var args = new MessageEventArgs("test");
            await x.RunAll(args);
            Assert.Equal(1, counter);
            x.Remove(handler);
            await x.RunAll(args);
            Assert.Equal(1, counter);
        }

        [Fact]
        public async void AddRemoveHandlerRegistry()
        {
            CreateMessageContext();

            var document = DomOperationsTesting.CreateDocument();
            var x = new MessageRegistry(document);
            int counter = 0;
            Task handler(MessageEventArgs args)
            {
                counter++;
                return Task.CompletedTask;
            }
            x.Add("a", handler);
            await document.Head.NotifyEvent("_a");
            Assert.Equal(1, counter);

            x.Remove("b", handler);
            await document.Head.NotifyEvent("_a");
            Assert.Equal(2, counter);

            x.Remove("a", handler);
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
            context.Setup(x => x.Application).Returns(_context.Application);
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
            int counter = 0;
            Task handler(MessageEventArgs args)
            {
                counter++;
                return Task.CompletedTask;
            }
            doc.AddMessageListener("a", handler);
            doc.Head.NotifyEvent("_a");
            Assert.Equal(1, counter);
            doc.RemoveMessageListener("a", handler);
            doc.Head.NotifyEvent("_a");
            Assert.Equal(1, counter);
        }

        [Fact]
        public void DocumentOnMessageRuns()
        {
            CreateMessageContext();
            var doc = DomOperationsTesting.CreateDocument();
            int counter = 0;
            Task handler()
            {
                counter++;
                return Task.CompletedTask;
            }
            doc.OnMessage("a", handler);
            doc.Head.NotifyEvent("_a");
            Assert.Equal(1, counter);
        }

        [Fact]
        public async void AsyncEventDispatches()
        {
            int counter = 0;
            var ev = new AsyncEvent();
            Task myMethod()
            {
                counter++;
                return Task.CompletedTask;
            }
            ev.Subscribe(myMethod);
            await ev.InvokeAsync(this, new EventArgs());
            Assert.Equal(1, counter);
            
            ev.Unsubscribe(myMethod);
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
            int counter = 0;
            Task myMethod(object sender, EventArgs args)
            {
                counter++;
                return Task.CompletedTask;
            };
            var handler = new AsyncEventHandler<EventArgs>(myMethod);
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
            int counter = 0;
            void myMethod() => counter++;
            var ev = new AsyncEvent<EventArgs>();
            ev.Subscribe(myMethod);
            await ev.InvokeAsync(this, new EventArgs());
            Assert.Equal(1, counter);

            ev.Unsubscribe(myMethod);
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
