/*
Copyright (c) 2019 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.DOM
{
    public class EventsTesting
    {
        [Fact]
        public void MessageEventsCreate()
        {
            var x = new MessageEventArgs("test");
            Assert.Equal("test", x.Body);
        }

        [Fact]
        public async void AddRemoveHandler()
        {
            var x = new MessageTypeRegistry();
            int counter = 0;
            Task handler(MessageEventArgs x)
            {
                Assert.Equal("test", x.Body);
                counter++;
                return Task.CompletedTask;
            }
            x.Add(handler);
            await x.RunAll("test");
            Assert.Equal(1, counter);
            x.Remove(handler);
            await x.RunAll("test");
            Assert.Equal(1, counter);
        }

        [Fact]
        public async void AddRemoveHandlerRegistry()
        {
            CreateMessageContext();

            var document = DomOperationsTesting.CreateDocument();
            var x = new MessageRegistry(document);
            int counter = 0;
            Task handler(MessageEventArgs x)
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

        private static void CreateMessageContext()
        {
            var context = new Mock<IPageContext>();
            LaraUI.InternalContext.Value = context.Object;
            var bridge = new Mock<IJSBridge>();
            context.Setup(x => x.JSBridge).Returns(bridge.Object);
            bridge.Setup(x => x.EventData).Returns("test");
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
    }
}
