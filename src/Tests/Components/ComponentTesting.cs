/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.Components
{
    public class ComponentTesting
    {
        [Fact]
        public void RegisterComponentSucceeds()
        {
            LaraUI.Publish(new WebComponentOptions
            {
                ComponentTagName = "x-com",
                ComponentType = typeof(MyComponent)
            });
            Assert.True(LaraUI.TryGetComponent("x-com", out var type));
            LaraUI.UnPublishWebComponent("x-com");
            Assert.Equal(typeof(MyComponent), type);
            Assert.False(LaraUI.TryGetComponent("x-com", out _));
        }

        class MyComponent : WebComponent
        {
            public MyComponent() : base("x-com")
            {
            }
        }

        class MyPage : IPage
        {
            public Task OnGet()
            {
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void ServerEventsOnSucceeds()
        {
            var context = CreateMockPage();
            Assert.Equal(ServerEventsStatus.Disabled, context.Document.ServerEventsStatus);
            context.JSBridge.ServerEventsOn();
            Assert.Equal(ServerEventsStatus.Connecting, context.Document.ServerEventsStatus);
            context.JSBridge.ServerEventsOff();
            Assert.Equal(ServerEventsStatus.Disabled, context.Document.ServerEventsStatus);
        }

        private PageContext CreateMockPage()
        {
            var http = new Mock<HttpContext>();
            var guid = Guid.Parse("{5166FB58-FB45-4622-90E6-195E2448F2C9}");
            var connection = new Connection(guid, IPAddress.Loopback);
            var page = new MyPage();
            var document = new Document(page);
            return new PageContext(http.Object, connection, document);
        }

        [Fact]
        public void WebComponentListsAllDescendents()
        {
            var x = new XCOM();
            var div = Element.Create("div");
            div.Id = "lala";
            x.AppendChild(div);
            var set = new HashSet<string>();
            foreach (var node in GetAllDescendents(x))
            {
                if (node is Element child && !string.IsNullOrEmpty(child.Id))
                {
                    set.Add(child.Id);
                }
            }
            Assert.Contains("div1", set);
            Assert.Contains("div2", set);
            Assert.Contains("div1a", set);
            Assert.Contains("lala", set);
        }

        [Fact]
        public void FlattenedChildrenIncludesPrintedOnes()
        {
            var container = Element.Create("div");
            var x = new XCOM();
            var div = Element.Create("div");
            div.Id = "lala";
            x.AppendChild(div);
            container.AppendChild(x);
            var set = new HashSet<string>();
            foreach (var node in GetFlattened(container))
            {
                if (node is Element child && !string.IsNullOrEmpty(child.Id))
                {
                    set.Add(child.Id);
                }
            }
            Assert.Contains("div1", set);
            Assert.Contains("div2", set);
            Assert.Contains("div1a", set);
            Assert.DoesNotContain("lala", set);
        }

        private IEnumerable<Node> GetAllDescendents(Element element)
        {
            return RecursiveExtension(element, x => x.GetAllDescendants());
        }

        private IEnumerable<Node> GetFlattened(Element element)
        {
            return RecursiveExtension(element, x => x.GetFlattenedChildren());
        }

        private IEnumerable<Node> RecursiveExtension(Element root, Func<Element, IEnumerable<Node>> method)
        {
            foreach (var node in method(root))
            {
                yield return node;
                if (node is Element child)
                {
                    foreach (var grandchild in RecursiveExtension(child, method))
                    {
                        yield return grandchild;
                    }
                }
            }
        }

        [LaraWebComponent("x-com")]
        class XCOM : WebComponent
        {
            public XCOM() : base("x-com")
            {
                AttachShadow();
                var builder = new LaraBuilder(ShadowRoot);
                builder.Push("div", "", "div1")
                    .Push("div", "", "div1a")
                    .Pop()
                .Pop()
                .Push("div", "", "div2")
                .Pop()
                .AddTextNode("lalas");
            }
        }

        class LightCom : WebComponent
        {
            public LightCom() : base("x-light")
            {
            }
        }

        [Fact]
        public void ElementWithoutShadowYieldsItself()
        {
            var x = new LightCom();
            var list = new List<Node>(x.GetLightSlotted());
            Assert.Single(list);
            Assert.Same(x, list[0]);
        }

        [Fact]
        public void GetSlotElementFinds()
        {
            var x = new MySlotter();
            var builder = new LaraBuilder(x);
            builder.Push("div", "", "slot1")
            .Pop()
            .Push("div", "", "slot2")
                .Push("div", "", "slot2a")
                .Pop()
            .Pop()
            .Push("div", "", "slot3")
                .Attribute("slot", "a")
            .Pop();
            var set = new HashSet<string>();
            foreach (var item in x.GetSlottedElements(""))
            {
                if (item is Element element)
                {
                    set.Add(element.Id);
                }
            }
            Assert.Contains("slot1", set);
            Assert.Contains("slot2", set);
            Assert.DoesNotContain("slot2a", set);
            Assert.DoesNotContain("slot3", set);
            var list = new List<Node>(x.GetSlottedElements("a"));
            Assert.Single(list);
            var child = list[0] as Element;
            Assert.Equal("slot3", child.Id);            
        }

        class MySlotter : WebComponent
        {
            public MySlotter() : base("x-slotter")
            {
                AttachShadow();
            }
        }

        [Fact]
        public void ComponentNotifiedAttributeChanged()
        {
            var x = new MyAttributeSubscriptor();
            x.SetAttribute("data-lala", "lolo");
            Assert.Equal("lolo", x.MyData);
        }

        class MyAttributeSubscriptor : WebComponent
        {
            public MyAttributeSubscriptor() : base("x-att")
            {
            }

            public string MyData { get; set; }

            protected override IEnumerable<string> GetObservedAttributes()
            {
                return new string[] { "data-lala" };
            }

            protected override void OnAttributeChanged(string attribute)
            {
                if (attribute == "data-lala")
                {
                    MyData = GetAttribute("data-lala");
                }
            }
        }

        [Fact]
        public void ObservedOnlyAttributeDoesNothing()
        {
            var x = new MyDummyComponent
            {
                Class = "lala"
            };
            Assert.Equal("lala", x.Class);
        }

        class MyDummyComponent : WebComponent
        {
            public MyDummyComponent() : base("x-dummy")
            {
            }

            protected override IEnumerable<string> GetObservedAttributes()
            {
                return new string[] { "class" };
            }
        }

        [Fact]
        public void PublishAssembliesComponent()
        {
            LaraUI.PublishAssemblies();

            Assert.True(LaraUI.TryGetComponent("x-com", out var type));
            Assert.Same(typeof(XCOM), type);
        }
    }
}
