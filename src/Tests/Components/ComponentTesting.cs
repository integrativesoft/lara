/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Components;
using Integrative.Lara.Delta;
using Integrative.Lara.DOM;
using Integrative.Lara.Main;
using Integrative.Lara.Tests.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.Components
{
    public class ComponentTesting
    {
        public ComponentTesting()
        {
            PublishHelper.PublishIfNeeded();
        }

        [Fact]
        public void RegisterComponentSucceeds()
        {
            LaraUI.Publish(new WebComponentOptions
            {
                ComponentTagName = "x-caca",
                ComponentType = typeof(MyComponent)
            });
            Assert.True(LaraUI.TryGetComponent("x-caca", out var type));
            LaraUI.UnPublishWebComponent("x-caca");
            Assert.Equal(typeof(MyComponent), type);
            Assert.False(LaraUI.TryGetComponent("x-caca", out _));
        }

        class MyComponent : WebComponent
        {
            public MyComponent() : base("x-caca")
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
            return RecursiveExtension(element, x => x.GetLightChildren());
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
                var builder = new LaraBuilder(ShadowRoot);
                builder.Push("div", "", "div1")
                    .Push("div", "", "div1a")
                    .Pop()
                .Pop()
                .Push("div", "", "div2")
                .Pop()
                .AppendText("lalas");
            }
        }

        [LaraWebComponent("x-light")]
        class LightCom : WebComponent
        {
            public LightCom() : base("x-light")
            {
            }
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

        [LaraWebComponent("x-slotter")]
        class MySlotter : WebComponent
        {
            public MySlotter() : base("x-slotter")
            {
                var div = Create("div");
                var slot = Create("slot");
                ShadowRoot.AppendChild(div);
                div.AppendChild(slot);
            }
        }

        [Fact]
        public void ComponentNotifiedAttributeChanged()
        {
            LaraUI.Publish(new WebComponentOptions
            {
                ComponentTagName = "x-att",
                ComponentType = typeof(MyAttributeSubscriptor)
            });
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

        [LaraWebComponent("x-dummy")]
        class MyDummyComponent : WebComponent
        {
            public int Moved { get; private set; }

            public MyDummyComponent() : base("x-dummy")
            {
            }

            protected override IEnumerable<string> GetObservedAttributes()
            {
                return new string[] { "class" };
            }

            protected override void OnMove()
            {
                base.OnMove();
                Moved++;
            }
        }

        [Fact]
        public void PublishAssembliesComponent()
        {
            Assert.True(LaraUI.TryGetComponent("x-com", out var type));
            Assert.Same(typeof(XCOM), type);
        }

        [Fact]
        public void SlotsPrintHostElements()
        {
            var document = new Document(new MyPage());
            var builder = new LaraBuilder(document.Body);
            builder.Push("x-slotter")
                .Push("div", "lalala")
                    .AppendText("hello")
                .Pop()
            .Pop();
            var writer = new DocumentWriter(document);
            writer.Print();
            var html = writer.ToString();
            Assert.Contains("lalala", html);
            Assert.Contains("hello", html);
            Assert.DoesNotContain("x-slotter", html);
        }

        [Fact]
        public void OrphanSlotPrintsItself()
        {
            var document = new Document(new MyPage());
            var builder = new LaraBuilder(document.Body);
            builder.Push("slot", "lalala").Pop();
            var writer = new DocumentWriter(document);
            writer.Print();
            var html = writer.ToString();
            Assert.Contains("lalala", html);
        }

        [Fact]
        public void SlotNameSetsAttribute()
        {
            var x = new Slot
            {
                Name = "lala"
            };
            Assert.Equal("lala", x.Name);
            Assert.Equal("lala", x.GetAttribute("name"));
        }

        [Fact]
        public void WebComponentsRequireDash()
        {
            var registry = new ComponentRegistry();
            bool found = false;
            try
            {
                registry.Register("baba", typeof(MyComponent));
            }
            catch (ArgumentException)
            {
                found = true;
            }
            Assert.True(found);
        }

        [Fact]
        public void WebComponentTypeRequired()
        {
            var registry = new ComponentRegistry();
            bool found = false;
            try
            {
                registry.Register("x-lala", null);
            }
            catch (ArgumentNullException)
            {
                found = true;
            }
            Assert.True(found);
        }

        [Fact]
        public void WebComponentsMustInherit()
        {
            var registry = new ComponentRegistry();
            bool found = false;
            try
            {
                registry.Register("x-lolo", typeof(InputElement));
            }
            catch (InvalidOperationException)
            {
                found = true;
            }
            Assert.True(found);
        }

        [Fact]
        public void CannotRegisterSameTagTwice()
        {
            var registry = new ComponentRegistry();
            registry.Register("x-baba", typeof(MyComponent));
            bool found = false;
            try
            {
                registry.Register("x-baba", typeof(MyComponent));
            }
            catch (InvalidOperationException)
            {
                found = true;
            }
            Assert.True(found);
        }

        [Fact]
        public void ShadowLightEmpty()
        {
            var shadow = new Shadow();
            shadow.AppendText("bah");
            Assert.Empty(shadow.GetLightSlotted());
        }

        [Fact]
        public void VerifyComponentRegistered()
        {
            bool blown = false;
            try
            {
                new MyUnregisteredComponent();
            }
            catch (InvalidOperationException)
            {
                blown = true;
            }
            Assert.True(blown);
        }

        class MyUnregisteredComponent : WebComponent
        {
            public MyUnregisteredComponent() : base("x-cocos")
            {
            }
        }

        [Fact]
        public void VerifyComponentSameType()
        {
            Assert.False(WebComponent.VerifyType("x-com", typeof(MyComponent), out _));
        }

        [Fact]
        public void NotifyMovedCalledDirectly()
        {
            var document = new Document(new MyPage());
            var x = new MyDummyComponent();
            var div1 = Element.Create("div");
            var div2 = Element.Create("div");
            document.Body.AppendChild(div1);
            document.Body.AppendChild(div2);
            div1.AppendChild(x);
            Assert.Equal(0, x.Moved);
            div2.AppendChild(x);
            Assert.Equal(1, x.Moved);
        }

        [Fact]
        public void GetContentNodeReturnsShadowChildren()
        {
            var component = new MyTwoDivComponent(true);
            var content = component.GetContentNode();
            Assert.Equal(ContentNodeType.Array, content.Type);
            var array = content as ContentArrayNode;
            Assert.NotNull(array);
            Assert.Equal(2, array.Nodes.Count);
            Assert.Equal(ContentNodeType.Element, array.Nodes[0].Type);
            Assert.Equal(ContentNodeType.Text, array.Nodes[1].Type);
        }

        [LaraWebComponent("x-twodiv")]
        class MyTwoDivComponent : WebComponent
        {
            public MyTwoDivComponent(bool useShadow) : base("x-twodiv")
            {
                if (useShadow)
                {
                    ShadowRoot.AppendChild(Create("div"));
                    ShadowRoot.AppendText("hello");
                }
            }
        }
    }
}
