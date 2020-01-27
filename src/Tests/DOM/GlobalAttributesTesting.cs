/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tests.Main;
using Integrative.Lara.Tests.Middleware;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace Integrative.Lara.Tests.DOM
{
    public class GlobalAttributesTesting : DummyContextTesting
    {
        [Fact]
        public void AccessKey()
        {
            var x = new Button
            {
                AccessKey = "1",
                Class = "2",
                ContentEditable = "3",
                Dir = "4",
                Draggable = "5",
                DropZone = "6",
                Hidden = true,
                Id = "7",
                Lang = "8",
                Spellcheck = "9",
                Style = "10",
                TabIndex = "11",
                Title = "12",
                Translate = "13"
            };
            Assert.Equal("1", x.GetAttribute("accesskey"));
            Assert.Equal("2", x.GetAttribute("class"));
            Assert.Equal("3", x.GetAttribute("contenteditable"));
            Assert.Equal("4", x.GetAttribute("dir"));
            Assert.Equal("5", x.GetAttribute("draggable"));
            Assert.Equal("6", x.GetAttribute("dropzone"));
            Assert.Equal("7", x.GetAttribute("id"));
            Assert.Equal("8", x.GetAttribute("lang"));
            Assert.Equal("9", x.GetAttribute("spellcheck"));
            Assert.Equal("10", x.GetAttribute("style"));
            Assert.Equal("11", x.GetAttribute("tabindex"));
            Assert.Equal("12", x.GetAttribute("title"));
            Assert.Equal("13", x.GetAttribute("translate"));
            Assert.True(x.Hidden);
            Assert.Equal("1", x.AccessKey);
            Assert.Equal("2", x.Class);
            Assert.Equal("3", x.ContentEditable);
            Assert.Equal("4", x.Dir);
            Assert.Equal("5", x.Draggable);
            Assert.Equal("6", x.DropZone);
            Assert.Equal("7", x.Id);
            Assert.Equal("8", x.Lang);
            Assert.Equal("9", x.Spellcheck);
            Assert.Equal("10", x.Style);
            Assert.Equal("11", x.TabIndex);
            Assert.Equal("12", x.Title);
            Assert.Equal("13", x.Translate);
        }

        [Fact]
        public void ElementToString()
        {
            var x = new Button();
            Assert.Equal("button", x.ToString());
            x.Id = "hi";
            Assert.Equal("button #hi", x.ToString());
        }

        [Fact]
        public void ClearingIdRemovesAttribute()
        {
            var x = Element.Create("button", "mybutton");
            Assert.Equal(NodeType.Element, x.NodeType);
            Assert.True(x.HasAttribute("id"));
            x.Id = null;
            Assert.False(x.HasAttribute("id"));
        }

        [Fact]
        public void SetAttributeId()
        {
            var x = Element.Create("button");
            x.SetAttribute("ID", "x");
            Assert.Equal("x", x.Id);
        }

        [Fact]
        public void RemoveAttributeId()
        {
            var x = Element.Create("span", "x");
            x.RemoveAttribute("id");
            Assert.True(string.IsNullOrEmpty(x.Id));            
        }

        [Fact]
        public void GetChildPositionNotFound()
        {
            var x1 = Element.Create("span");
            var x2 = Element.Create("span");
            int index = x1.GetChildNodePosition(x2);
            Assert.Equal(-1, index);
        }

        [Fact]
        public void GrandchildDescendsFromElement()
        {
            var x1 = Element.Create("span");
            var x2 = Element.Create("span");
            var x3 = Element.Create("span");
            x1.AppendChild(x2);
            x2.AppendChild(x3);
            Assert.True(x3.DescendsFrom(x1));
        }

        [Fact]
        public void InsertChildAfter()
        {
            var div = Element.Create("div");
            var x1 = Element.Create("span");
            var x2 = Element.Create("span");
            var x3 = Element.Create("span");
            div.AppendChild(x1);
            div.AppendChild(x3);
            div.InsertChildAfter(x1, x2);
            var list = new List<Node>(div.Children);
            Assert.Equal(3, list.Count);
            Assert.Same(x2, list[1]);
        }

        [Fact]
        public void GetContentNodeElement()
        {
            var div = Element.Create("div", "mydiv");
            var span = Element.Create("span");
            var text = Element.Create("hello");
            div.AppendChild(span);
            span.AppendChild(text);
            var content = div.GetContentNode();
            Assert.True(content is ContentElementNode);
            var ce = (ContentElementNode)content;
            Assert.Equal("div", ce.TagName);
            Assert.Single(ce.Children);
            Assert.Single(ce.Attributes);
            var att = ce.Attributes![0];
            Assert.Equal("id", att.Attribute);
            Assert.Equal("mydiv", att.Value);
        }

        [Fact]
        public void InlineChildElementsPrintedInline()
        {
            var doc = new Document(new MyPage(), BaseModeController.DefaultKeepAliveInterval);
            var b = Element.Create("span");
            doc.Body.AppendChild(b);
            doc.Body.AppendChild(new TextNode("hello"));
            var writer = new DocumentWriter(doc);
            writer.Print();
            string result = writer.ToString();
            Assert.Contains("/span>hello", result);
        }

        [Fact]
        public void AddDuplicateIdThrows()
        {
            var map = new DocumentIdMap();
            var a1 = Element.Create("span", "a");
            var a2 = Element.Create("span", "a");
            map.NotifyAdded(a1);
            DomOperationsTesting.Throws<DuplicateElementIdException>(() => map.NotifyAdded(a2));
        }

        [Fact]
        public void CheckedFalseFlushed()
        {
            var doc = new Document(new MyPage(), BaseModeController.DefaultKeepAliveInterval);
            var x = new InputElement
            {
                Id = "x"
            };
            doc.Body.AppendChild(x);
            x.Checked = true;
            doc.OpenEventQueue();
            x.Checked = false;
            var queue = doc.GetQueue();
            Assert.NotEmpty(queue);
            var top = queue.Peek() as SetCheckedDelta;
            Assert.NotNull(top);
            Assert.Equal(x.Id, top!.ElementId);
            Assert.False(top.Checked);
        }

        [Fact]
        public void CheckedTrueFlushed()
        {
            var doc = new Document(new MyPage(), BaseModeController.DefaultKeepAliveInterval);
            var x = new InputElement
            {
                Id = "x"
            };
            doc.Body.AppendChild(x);
            doc.OpenEventQueue();
            x.Checked = true;
            var queue = doc.GetQueue();
            Assert.NotEmpty(queue);
            var top = queue.Peek() as SetCheckedDelta;
            Assert.NotNull(top);
            Assert.Equal(x.Id, top!.ElementId);
            Assert.True(top.Checked);
        }

        [Fact]
        public void NotifyCheckedTrueAdds()
        {
            var div = Element.Create("div");
            var a = new Attributes(div);
            a.NotifyChecked(true);
            Assert.True(a.HasAttribute("checked"));
            IEnumerator e = ((IEnumerable)a).GetEnumerator();
            Assert.True(e.MoveNext());
            a.NotifyChecked(false);
            Assert.False(a.HasAttribute("checked"));
        }
    }
}
