/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.DOM
{
    public class BuilderTesting
    {
        [Fact]
        public void PushAdds()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.Push("button", "red", "mybutton");
            Assert.NotEmpty(root.Children);
            var first = root.Children.FirstOrDefault() as Button;
            Assert.NotNull(first);
            Assert.Equal("red", first.Class);
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
            builder.AddTextNode("&lt;");
            var node = root.Children.FirstOrDefault() as TextNode;
            Assert.NotNull(node);
            Assert.Equal("&amp;lt;", node.Data);
        }

        [Fact]
        public void AddElements()
        {
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            var list = new List<Element>()
            {
                new Button(),
                new Option()
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
                new Option()
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
            bool executed = false;
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.On(new EventSettings
            {
                EventName = "click",
                Handler = app =>
                {
                    executed = true;
                    return Task.CompletedTask;
                }
            });
            var http = new Mock<HttpContext>();
            var page = new Mock<IPage>();
            var context = new PageContext(http.Object, new Document(page.Object));
            await root.NotifyEvent("click", context);
            Assert.True(executed);
        }

        [Fact]
        public async void OnEventSimple()
        {
            bool executed = false;
            var root = Element.Create("div");
            var builder = new LaraBuilder(root);
            builder.On("click", app =>
            {
                executed = true;
                return Task.CompletedTask;
            });
            var http = new Mock<HttpContext>();
            var page = new Mock<IPage>();
            var context = new PageContext(http.Object, new Document(page.Object));
            await root.NotifyEvent("click", context);
            Assert.True(executed);
        }
    }
}
