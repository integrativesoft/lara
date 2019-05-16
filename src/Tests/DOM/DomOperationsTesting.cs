/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Delta;
using Integrative.Clara.DOM;
using Integrative.Clara.Main;
using Integrative.Clara.Tests.Main;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Clara.Tests.DOM
{
    public class DomOperationsTesting
    {
        readonly Func<IPageContext, Task> _emptyHandler;

        public DomOperationsTesting()
        {
            _emptyHandler = (app => Task.CompletedTask);
        }
        
        [Fact]
        public void AddElementWithId()
        {
            var button = new Element("button", "mybutton");
            var document = CreateDocument();
            document.Body.AppendChild(button);
            Assert.True(document.TryGetElementById("mybutton", out var found));
            Assert.Same(button, found);
        }

        private Document CreateDocument()
        {
            var guid = Connections.CreateCryptographicallySecureGuid();
            var page = new MyPage();
            return new Document(page, guid);
        }

        [Fact]
        public void AddBranchWithId()
        {
            var button = new Element("button", "mybutton");
            var span = new Element("span", "myspan");
            button.AppendChild(span);
            var document = CreateDocument();
            document.Body.AppendChild(button);
            Assert.True(document.TryGetElementById("myspan", out var found));
            Assert.Same(span, found);
        }

        [Fact]
        public void RemoveElementWithId()
        {
            var button = new Element("button", "mybutton");
            var document = CreateDocument();
            document.Body.AppendChild(button);
            button.Remove();
            Assert.False(document.TryGetElementById("mybutton", out _));
        }

        [Fact]
        public void RemoveBranchWithIdInside()
        {
            var button = new Element("button");
            var span = new Element("span", "myspan");
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
            var button1 = new Element("button", "mybutton");
            var button2 = new Element("button", "mybutton");
            var div = new Element("div");
            div.AppendChild(button1);
            div.AppendChild(button2);
            var doc = CreateDocument();
            Throws<InvalidOperationException>(() => doc.Body.AppendChild(div));
        }

        [Fact]
        public void CannotInsertDuplicateId()
        {
            var button1 = new Element("button", "mybutton");
            var button2 = new Element("button", "mybutton");
            var div = new Element("div");
            div.AppendChild(button1);
            div.AppendChild(button2);
            var doc = CreateDocument();
            var pane = new Element("div");
            doc.Body.AppendChild(pane);
            Throws<InvalidOperationException>(() => doc.Body.InsertChildAfter(pane, div));
        }

        [Fact]
        public void CannotAddNodeInsideItself()
        {
            var e1 = new Element("span");
            var e2 = new Element("span");
            e1.AppendChild(e2);
            Throws<InvalidOperationException>(() => e2.AppendChild(e1));
        }

        private void Throws<T>(Action action) where T : Exception
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
            var div = new Element("div");
            var span1 = new Element("span");
            var span2 = new Element("span");
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
            var span = new Element("span");
            span.On("click", _emptyHandler);
            var div = new Element("div");
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
            var span = new Element("span");
            span.On("click", _emptyHandler);
            var div = new Element("div");
            div.On("click", _emptyHandler);
            div.AppendChild(span);
            var dummy = new Element("button");
            var doc = CreateDocument();
            doc.Body.AppendChild(dummy);
            doc.Body.InsertChildBefore(dummy, div);
            Assert.False(string.IsNullOrEmpty(span.Id));
            Assert.False(string.IsNullOrEmpty(div.Id));
        }

        [Fact]
        public void ElementRemoveHandler()
        {
            var x = new Element("button");
            x.On("click", _emptyHandler);
            Assert.True(x.HasAttribute("onclick"));
            x.On("click", null);
            Assert.False(x.HasAttribute("onclick"));
        }

        [Fact]
        public void TransferElementBetweenDocuments()
        {
            var button = new Element("button", "mybutton");
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
            var div = new Element("div", "mydiv");
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
            var div = new Element("div", "mydiv");
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
            var div = new Element("div", "mydiv");
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
            var div = new Element("div");
            var doc = CreateDocument();
            doc.Body.AppendChild(div);
            doc.OpenEventQueue();
            div.SetAttribute("data-test", "x");
            var queue = doc.GetQueue();
            Assert.NotEmpty(queue);
            var step = queue.Peek() as SetIdDelta;
            Assert.NotNull(step);
            Assert.Equal(doc.Body.Id, step.Locator.StartingId);
            Assert.Equal(div.Id, step.NewId);
        }
    }
}
