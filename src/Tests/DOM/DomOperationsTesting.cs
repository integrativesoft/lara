/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using Integrative.Lara.DOM;
using Integrative.Lara.Main;
using Integrative.Lara.Tests.Main;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.DOM
{
    public class DomOperationsTesting
    {
        readonly Func<Task> _emptyHandler;

        public DomOperationsTesting()
        {
            _emptyHandler = (() => Task.CompletedTask);
        }
        
        [Fact]
        public void AddElementWithId()
        {
            var button = Element.Create("button", "mybutton");
            var document = CreateDocument();
            document.Body.AppendChild(button);
            Assert.True(document.TryGetElementById("mybutton", out var found));
            Assert.Same(button, found);
        }

        private Document CreateDocument()
        {
            var guid = Connections.CreateCryptographicallySecureGuid();
            var page = new MyPage();
            return new Document(page, guid, new LaraOptions());
        }

        [Fact]
        public void AddBranchWithId()
        {
            var button = Element.Create("button", "mybutton");
            var span = Element.Create("span", "myspan");
            button.AppendChild(span);
            var document = CreateDocument();
            document.Body.AppendChild(button);
            Assert.True(document.TryGetElementById("myspan", out var found));
            Assert.Same(span, found);
        }

        [Fact]
        public void RemoveElementWithId()
        {
            var button = Element.Create("button", "mybutton");
            var document = CreateDocument();
            document.Body.AppendChild(button);
            button.Remove();
            Assert.False(document.TryGetElementById("mybutton", out _));
        }

        [Fact]
        public void RemoveBranchWithIdInside()
        {
            var button = Element.Create("button");
            var span = Element.Create("span", "myspan");
            button.AppendChild(span);
            var doc = CreateDocument();
            doc.Body.AppendChild(button);
            button.Remove();
            Assert.False(doc.TryGetElementById("myspan", out _));
        }

        [Fact]
        public void CannotRemoveDocumentHead()
        {
            var doc = CreateDocument();
            Assert.Throws<InvalidOperationException>(() => doc.Head.Remove());
        }

        [Fact]
        public void CannotRemoveDocumentBody()
        {
            var doc = CreateDocument();
            Assert.Throws<InvalidOperationException>(() => doc.Body.Remove());
        }

        [Fact]
        public void CannotAddDuplicateId()
        {
            var button1 = Element.Create("button", "mybutton");
            var button2 = Element.Create("button", "mybutton");
            var div = Element.Create("div");
            div.AppendChild(button1);
            div.AppendChild(button2);
            var doc = CreateDocument();
            Throws<InvalidOperationException>(() => doc.Body.AppendChild(div));
        }

        [Fact]
        public void CannotInsertDuplicateId()
        {
            var button1 = Element.Create("button", "mybutton");
            var button2 = Element.Create("button", "mybutton");
            var div = Element.Create("div");
            div.AppendChild(button1);
            div.AppendChild(button2);
            var doc = CreateDocument();
            var pane = Element.Create("div");
            doc.Body.AppendChild(pane);
            Throws<InvalidOperationException>(() => doc.Body.InsertChildAfter(pane, div));
        }

        [Fact]
        public void CannotAddNodeInsideItself()
        {
            var e1 = Element.Create("span");
            var e2 = Element.Create("span");
            e1.AppendChild(e2);
            Throws<InvalidOperationException>(() => e2.AppendChild(e1));
        }

        internal static void Throws<T>(Action action) where T : Exception
        {
            bool error = false;
            try
            {
                action();
            }
            catch (T)
            {
                error = true;
            }
            Assert.True(error);
        }

        internal static async Task ThrowsAsync<T>(Func<Task> action) where T : Exception
        {
            bool error = false;
            try
            {
                await action();
            }
            catch (T)
            {
                error = true;
            }
            Assert.True(error);
        }

        [Fact]
        public void TextNodeContent()
        {
            var node = new TextNode("hello");
            var x = node.GetContentNode();
            Assert.Equal(NodeType.Text, node.NodeType);
            Assert.Equal(ContentNodeType.Text, x.Type);
            Assert.True(x is ContentTextNode);
            Assert.Equal("hello", ((ContentTextNode)x).Data);
        }

        [Fact]
        public void InsertBeforeInserts()
        {
            var div = Element.Create("div");
            var span1 = Element.Create("span");
            var span2 = Element.Create("span");
            div.AppendChild(span2);
            div.InsertChildBefore(span2, span1);
            var list = new List<Node>(div.Children);
            Assert.NotEmpty(list);
            Assert.Equal(2, list.Count);
            Assert.Equal(2, div.ChildCount);
            Assert.Same(span1, list[0]);
            Assert.Same(span2, list[1]);
        }

        [Fact]
        public void GenerateIdsForEvents()
        {
            var span = Element.Create("span");
            span.On("click", _emptyHandler);
            var div = Element.Create("div");
            div.On("click", _emptyHandler);
            div.AppendChild(span);
            var doc = CreateDocument();
            doc.Body.AppendChild(div);
            Assert.False(string.IsNullOrEmpty(span.Id));
            Assert.False(string.IsNullOrEmpty(div.Id));
        }


        [Fact]
        public void GenerateIdsForEventsInsert()
        {
            var span = Element.Create("span");
            span.On("click", _emptyHandler);
            var div = Element.Create("div");
            div.On("click", _emptyHandler);
            div.AppendChild(span);
            var dummy = Element.Create("button");
            var doc = CreateDocument();
            doc.Body.AppendChild(dummy);
            doc.Body.InsertChildBefore(dummy, div);
            Assert.False(string.IsNullOrEmpty(span.Id));
            Assert.False(string.IsNullOrEmpty(div.Id));
        }

        [Fact]
        public void ElementRemoveHandler()
        {
            var x = Element.Create("button");
            x.On("click", _emptyHandler);
            Assert.True(x.HasAttribute("onclick"));
            x.On("click", null);
            Assert.False(x.HasAttribute("onclick"));
        }

        [Fact]
        public void TransferElementBetweenDocuments()
        {
            var button = Element.Create("button", "mybutton");
            var doc1 = CreateDocument();
            var doc2 = CreateDocument();
            doc1.Body.AppendChild(button);
            doc2.Body.AppendChild(button);
            Assert.False(doc1.TryGetElementById("mybutton", out _));
            Assert.True(doc2.TryGetElementById("mybutton", out var found));
            Assert.Same(button, found);
        }

        [Fact]
        public void RemoveNode()
        {
            var div = Element.Create("div", "mydiv");
            var doc = CreateDocument();
            doc.Body.AppendChild(new TextNode("hi"));
            doc.Body.AppendChild(div);
            doc.OpenEventQueue();
            div.Remove();
            var queue = doc.GetQueue();
            Assert.NotEmpty(queue);
            var step = queue.Peek() as NodeRemovedDelta;
            Assert.NotNull(step);
            Assert.Equal(doc.Body.Id, step.ParentId);
            Assert.Equal(1, step.ChildIndex);
        }

        [Fact]
        public void NodeAdded()
        {
            var div = Element.Create("div", "mydiv");
            var doc = CreateDocument();
            doc.OpenEventQueue();
            doc.Body.AppendChild(div);
            var queue = doc.GetQueue();
            Assert.NotEmpty(queue);
            var step = queue.Peek() as NodeAddedDelta;
            Assert.NotNull(step);
            Assert.Equal(doc.Body.Id, step.ParentId);
            var content = step.Node as ContentElementNode;
            Assert.NotNull(content);
            Assert.Equal("div", content.TagName);
            Assert.NotEmpty(content.Attributes);
            var att = content.Attributes[0];
            Assert.Equal("id", att.Attribute);
            Assert.Equal("mydiv", att.Value);
        }

        [Fact]
        public void NodeInsertedDelta()
        {
            var div = Element.Create("div", "mydiv");
            var doc = CreateDocument();
            var text = new TextNode("lala");
            doc.Body.AppendChild(text);
            doc.OpenEventQueue();
            doc.Body.InsertChildAfter(text, div);
            var queue = doc.GetQueue();
            Assert.NotEmpty(queue);
            var step = queue.Peek() as NodeInsertedDelta;
            Assert.NotNull(step);
            Assert.Equal(doc.Body.Id, step.ParentElementId);
            Assert.Equal(1, step.Index);
            var content = step.Node as ContentElementNode;
            Assert.NotNull(content);
            Assert.Equal("div", content.TagName);
            Assert.NotEmpty(content.Attributes);
            var att = content.Attributes[0];
            Assert.Equal("id", att.Attribute);
            Assert.Equal("mydiv", att.Value);
        }

        [Fact]
        public void SetIdOnAttribute()
        {
            var div1 = Element.Create("div");
            var div2 = Element.Create("div");
            var doc = CreateDocument();
            doc.Body.AppendChild(div1);
            div1.AppendChild(div2);
            doc.OpenEventQueue();
            div2.SetAttribute("data-test", "x");
            var queue = doc.GetQueue();
            Assert.NotEmpty(queue);
            var step = queue.Peek() as SetIdDelta;
            Assert.NotNull(step);
            Assert.Equal(div1.Id, step.Locator.StartingId);
            Assert.Equal(div2.Id, step.NewId);
        }

        [Fact]
        public void FocusFailsOnGet()
        {
            var div = Element.Create("div");
            Throws<InvalidOperationException>(() => div.Focus());
        }

        [Fact]
        public void RemoveOrphanThrows()
        {
            var div = Element.Create("div");
            Throws<InvalidOperationException>(() => div.Remove());
        }

        [Fact]
        public void InsertBeforeUnknownThrows()
        {
            var div1 = Element.Create("div");
            var div2 = Element.Create("div");
            var div3 = Element.Create("div");
            Throws<InvalidOperationException>(() => div1.InsertChildBefore(div2, div3));
        }

        [Fact]
        public void RemoveUnknownChildThrows()
        {
            var a = Element.Create("span");
            var b = Element.Create("span");
            Throws<InvalidOperationException>(() => a.RemoveChild(b));
        }

        [Fact]
        public void ClearChildrenRemovesThem()
        {
            var div = Element.Create("div");
            var span1 = Element.Create("span");
            var span2 = Element.Create("span");
            div.AppendChild(span1);
            div.AppendChild(span2);
            div.ClearChildren();
            Assert.Empty(div.Children);
            Assert.Equal(0, div.ChildCount);
        }

        [Fact]
        public void InsertAtSucceeds()
        {
            var x = Element.Create("div");
            var a = Element.Create("div");
            var b = Element.Create("div");
            var c = Element.Create("div");
            x.AppendChild(a);
            x.AppendChild(c);
            x.InsertChildAt(1, b);
            Assert.Equal(3, x.ChildCount);
            Assert.Same(b, x.GetChildAt(1));
        }

        [Fact]
        public void RemoveAtSucceeds()
        {
            var x = Element.Create("div");
            var a = Element.Create("div");
            var b = Element.Create("div");
            x.AppendChild(a);
            x.AppendChild(b);
            x.RemoveAt(1);
            Assert.Equal(1, x.ChildCount);
            Assert.Same(a, x.GetChildAt(0));
        }
    }
}
