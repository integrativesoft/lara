/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tests.Main;
using Integrative.Lara.Tests.Middleware;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.DOM
{
    public class DomOperationsTesting : DummyContextTesting
    {
        private readonly Func<Task> _emptyHandler;

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

        internal static Document CreateDocument()
        {
            var guid = Connections.CreateCryptographicallySecureGuid();
            var page = new MyPage();
            return new Document(page, guid, BaseModeController.DefaultKeepAliveInterval);
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
            Assert.Equal(doc.Body.Id, step!.ParentId);
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
            Assert.Equal(doc.Body.Id, step!.ParentId);
            var content = step.Node as ContentElementNode;
            Assert.NotNull(content);
            Assert.Equal("div", content!.TagName);
            Assert.NotEmpty(content.Attributes);
            var att = content.Attributes![0];
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
            Assert.Equal(doc.Body.Id, step!.ParentElementId);
            Assert.Equal(1, step.Index);
            var content = step.Node as ContentElementNode;
            Assert.NotNull(content);
            Assert.Equal("div", content!.TagName);
            Assert.NotEmpty(content.Attributes);
            var att = content.Attributes![0];
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
            Assert.Equal(div1.Id, step!.Locator!.StartingId);
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

        [Fact]
        public void MissingEventNameThrows()
        {
            var settings = new EventSettings();
            bool found = false;
            try
            {
                settings.Verify();
            }
            catch (ArgumentException)
            {
                found = true;
            }
            Assert.True(found);
        }

        [Fact]
        public void DocumentGetElementById()
        {
            var document = new Document(new MyPage(), BaseModeController.DefaultKeepAliveInterval);
            var div = Element.Create("div");
            div.Id = "lala";
            document.Body.AppendChild(div);
            var found = document.GetElementById("lala");
            Assert.Same(div, found);
        }

        [Fact]
        public async void DocumentOnUnloadExecutes()
        {
            int counter = 0;
            var document = new Document(new MyPage(), BaseModeController.DefaultKeepAliveInterval);
            document.OnUnload += (sender, args) => counter++;
            await document.NotifyUnload();
            Assert.Equal(1, counter);
        }

        [Fact]
        public void SwapChildrenSwaps()
        {
            var document = new Document(new MyPage(), BaseModeController.DefaultKeepAliveInterval);
            var div = Element.Create("div");
            var n1 = new TextNode("n1");
            var n2 = new TextNode("n2");
            var n3 = new TextNode("n3");
            div.AppendChild(n1);
            div.AppendChild(n2);
            div.AppendChild(n3);
            document.Body.AppendChild(div);
            document.OpenEventQueue();
            div.SwapChildren(1, 1);
            div.SwapChildren(0, 2);
            Assert.Equal(3, div.ChildCount);
            Assert.Same(n3, div.GetChildAt(0));
            Assert.Same(n1, div.GetChildAt(2));
            var queue = document.GetQueue();
            Assert.NotEmpty(queue);
            var first = queue.Peek() as SwapChildrenDelta;
            Assert.NotNull(first);
            Assert.Equal(div.Id, first!.ParentId);
            Assert.NotNull(div.Id);
            Assert.Equal(0, first.Index1);
            Assert.Equal(2, first.Index2);
        }

        [Fact]
        public void InputNotifyValueUpdates()
        {
            var input = new InputElement();
            input.NotifyValue(new ElementEventValue
            {
                Checked = true,
                ElementId = input.EnsureElementId(),
                Value = "a"
            });
            Assert.True(input.Checked);
            Assert.Equal("a", input.Value);
        }

        [Fact]
        // ReSharper disable once InconsistentNaming
        public void ElementGetChildPosition2nd()
        {
            var div = Element.Create("div");
            var x1 = Element.Create("div");
            var x2 = Element.Create("div");
            div.AppendChild(x1);
            div.AppendChild(x2);
            var index = div.GetChildElementPosition(x2);
            Assert.Equal(1, index);
        }

        [Fact]
        public void RemoveEventRemovesIt()
        {
            var div = Element.Create("div");
            div.On("click", () => Task.CompletedTask);
            div.On("click", null);
            Assert.Empty(div.Events);
        }

        [Fact]
        public void ElementAppendDataWorks()
        {
            var div = Element.Create("div");
            div.AppendData("@@");
            Assert.Equal(1, div.ChildCount);
            var child = div.GetChildAt(0) as TextNode;
            Assert.NotNull(child);
            Assert.Equal("@@", child!.Data);
        }

        private class DummyAdoptable : WebComponent
        {
            public int AdoptedCount { get; private set; }

            public DummyAdoptable() : base("x-adoptable")
            {
            }

            protected override void OnAdopted()
            {
                AdoptedCount++;
            }
        }

        [Fact]
        public void NotifyAdoptedPassedToChildren()
        {
            Context.Application.PublishComponent(new WebComponentOptions
            {
                ComponentTagName = "x-adoptable",
                ComponentType = typeof(DummyAdoptable)
            });
            var div = Element.Create("div");
            var x = new DummyAdoptable();
            div.AppendChild(x);
            var doc1 = CreateDocument();
            doc1.Body.AppendChild(div);
            var doc2 = CreateDocument();
            doc2.Body.AppendChild(div);
            Assert.Equal(1, x.AdoptedCount);
            Context.Application.UnPublishWebComponent("x-adoptable");
        }

        [Fact]
        public void SetInnerDataSetsData()
        {
            var div = Element.Create("div");
            div.SetInnerData("@@");
            Assert.Equal(1, div.ChildCount);
            var child = div.GetChildAt(0) as TextNode;
            Assert.NotNull(child);
            Assert.Equal("@@", child!.Data);
        }

        [Fact]
        public void SetInnerTextReplacesText()
        {
            var div = Element.Create("div");
            div.InnerText = "bb";
            div.InnerText = "a<a";
            Assert.Equal(1, div.ChildCount);
            var child = div.GetChildAt(0) as TextNode;
            Assert.NotNull(child);
            Assert.Equal("a&lt;a", child!.Data);
        }

        [Fact]
        public void SetInnerDataReplacesData()
        {
            var div = Element.Create("div");
            div.SetInnerData("a");
            div.SetInnerData("@@");
            Assert.Equal(1, div.ChildCount);
            var child = div.GetChildAt(0) as TextNode;
            Assert.NotNull(child);
            Assert.Equal("@@", child!.Data);
        }

        [Fact]
        public void RemoveEventYieldsDelta()
        {
            var doc = CreateDocument();
            var div = Element.Create("div");
            var counter = 0;
            Task Handler()
            {
                counter++;
                return Task.CompletedTask;
            }
            div.On("click", Handler);
            div.NotifyEvent("click");
            doc.Body.AppendChild(div);
            doc.FlushQueue();
            div.On("click", null);
            div.NotifyEvent("click");
            Assert.Equal(1, counter);
            var queue = doc.GetQueue();
            Assert.Single(queue);
            var first = queue.Peek() as UnsubscribeDelta;
            Assert.NotNull(first);
            Assert.Equal(div.Id, first!.ElementId);
            Assert.Equal("click", first.EventName);
        }

        [Fact]
        public void ElementGetHtml()
        {
            var div = Element.Create("div");
            var html = div.GetHtml();
            Assert.StartsWith("<div></div>", html);
        }

        [Fact]
        public void FocusEnqueues()
        {
            var doc = CreateDocument();
            var div = Element.Create("div");
            doc.Body.AppendChild(div);
            div.Focus();
            var q = doc.GetQueue();
            Assert.Single(q);
            var first = q.Peek() as FocusDelta;
            Assert.NotNull(first);
            Assert.Equal(div.EnsureElementId(), first!.ElementId);
        }

        [Fact]
        public void ButtonNotifyValue()
        {
            var x = new Button();
            x.NotifyValue(new ElementEventValue
            {
                ElementId = x.EnsureElementId(),
                Value = "test"
            });
            Assert.Equal("test", x.GetAttribute("value"));
        }

        [Fact]
        public void TextNodeAppendData()
        {
            var x = new TextNode();
            x.AppendData("a<a");
            Assert.Equal("a<a", x.Data);
        }

        [Fact]
        public void TextNodeAppendText()
        {
            var x = new TextNode();
            x.AppendText("a<a");
            Assert.Equal("a&lt;a", x.Data);
        }
    }
}
