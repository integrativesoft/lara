/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tests.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.DOM
{
    public class BuilderTesting : DummyContextTesting
    {
        [Fact]
        public void PushAdds()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.Push("button", "red", "mybutton").Pop();
            Assert.NotEmpty(root.Children);
            var first = root.Children.FirstOrDefault() as Button;
            Assert.NotNull(first);
            Assert.Equal("red", first!.Class);
            Assert.Equal("mybutton", first.Id);
        }

        [Fact]
        public void TooManyPops()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.Push("button", "red").Pop();
            DomOperationsTesting.Throws<InvalidOperationException>(() => builder.Pop());
        }

        [Fact]
        public void AddSiblings()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.Push("button", "red").Pop();
            builder.Push("button", "red").Pop();
            Assert.Equal(2, root.ChildCount);
        }

        [Fact]
        public void AddTextNodeEncodes()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.AppendText("&lt;");
            var node = root.Children.FirstOrDefault() as TextNode;
            Assert.NotNull(node);
            Assert.Equal("&amp;lt;", node!.Data);
        }

        [Fact]
        public void AddElements()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            var list = new List<Element>()
            {
                new Button(),
                new OptionElement()
            };
            builder.AddNodes(list);
            Assert.Equal(2, root.ChildCount);
        }

        [Fact]
        public void AddNodes()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            var list = new List<Node>()
            {
                new Button(),
                new OptionElement()
            };
            builder.AddNodes(list);
            Assert.Equal(2, root.ChildCount);
        }

        [Fact]
        public void AddAction()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.Add(MyAddAction);
            Assert.Equal(1, root.ChildCount);
        }

        private void MyAddAction(LaraBuilder builder)
        {
            builder.AddNode(new Button());
        }

        [Fact]
        public void SetAttribute()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.Attribute("class", "red");
            Assert.Equal("red", root.Class);
        }

        [Fact]
        public void SetFlag()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.FlagAttribute("hidden", true);
            Assert.True(root.Hidden);
        }

        [Fact]
        public async void OnEvent()
        {
            var executed = false;
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.On(new EventSettings
            {
                EventName = "click",
                Handler = () =>
                {
                    executed = true;
                    return Task.CompletedTask;
                }
            });
            /*var connection = new Connection(Guid.NewGuid(), IPAddress.Loopback);
            var http = new Mock<HttpContext>();
            var mock = new Mock<IPage>();
            var context = new PageContext(_context.Application,
                http.Object, connection);*/
            await root.NotifyEvent("click");
            Assert.True(executed);
        }

        [Fact]
        public async void OnEventSimple()
        {
            var executed = false;
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.On("click", () =>
            {
                executed = true;
                return Task.CompletedTask;
            });
            //var http = new Mock<HttpContext>();
            //var page = new Mock<IPage>();
            //var connection = new Connection(Guid.NewGuid(), IPAddress.Loopback);
            //var context = new PageContext(_context.Application, http.Object, connection);
            await root.NotifyEvent("click");
            Assert.True(executed);
        }

        [Fact]
        public void PushClassName()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.Push("div", "red").Pop();
            Assert.NotEmpty(root.Children);
            var child = root.Children.FirstOrDefault() as Element;
            Assert.NotNull(child);
            Assert.Equal("red", child!.Class);
        }

        [Fact]
        // ReSharper disable once InconsistentNaming
        public void PushNS()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.PushNS("abc", "svg").Pop();
            Assert.NotEmpty(root.Children);
            var child = root.Children.FirstOrDefault() as Element;
            Assert.NotNull(child);
            Assert.Equal("abc", child!.GetAttribute("xlmns"));
        }

        [Fact]
        public void PushElementClass()
        {
            var root = Element.Create("root");
            var div = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.Push(div, "red");
            Assert.Equal("red", div.Class);
        }

        [Fact]
        public void SessionIdAvailable()
        {
            var guid = Guid.Parse("{0F9EE9CD-F9A0-40E6-A91B-FE4E3E2282F0}");
            var cnx = new Connection(guid, IPAddress.Loopback);
            var x = new Session(cnx);
            Assert.Equal(guid, x.SessionId);
        }
    }
}
