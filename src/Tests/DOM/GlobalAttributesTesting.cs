/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Delta;
using Integrative.Clara.DOM;
using System.Collections.Generic;
using Xunit;

namespace Integrative.Clara.Tests.DOM
{
    public class GlobalAttributesTesting
    {
        [Fact]
        public void AccessKey()
        {
            var x = new Element("button")
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
            var x = new Element("button");
            Assert.Equal("button", x.ToString());
            x.Id = "hi";
            Assert.Equal("button id='hi'", x.ToString());
        }

        [Fact]
        public void ClearingIdRemovesAttribute()
        {
            var x = new Element("button", "mybutton");
            Assert.Equal(NodeType.Element, x.NodeType);
            Assert.True(x.HasAttribute("id"));
            x.Id = null;
            Assert.False(x.HasAttribute("id"));
        }

        [Fact]
        public void SetAttributeId()
        {
            var x = new Element("button");
            x.SetAttribute("ID", "x");
            Assert.Equal("x", x.Id);
        }

        [Fact]
        public void RemoveAttributeId()
        {
            var x = new Element("span", "x");
            x.RemoveAttribute("id");
            Assert.True(string.IsNullOrEmpty(x.Id));            
        }

        [Fact]
        public void GetChildPositionNotFound()
        {
            var x1 = new Element("span");
            var x2 = new Element("span");
            int index = x1.GetChildPosition(x2);
            Assert.Equal(-1, index);
        }

        [Fact]
        public void GrandchildDescendsFromElement()
        {
            var x1 = new Element("span");
            var x2 = new Element("span");
            var x3 = new Element("span");
            x1.AppendChild(x2);
            x2.AppendChild(x3);
            Assert.True(x3.DescendsFrom(x1));
        }

        [Fact]
        public void InsertChildAfter()
        {
            var div = new Element("div");
            var x1 = new Element("span");
            var x2 = new Element("span");
            var x3 = new Element("span");
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
            var div = new Element("div", "mydiv");
            var span = new Element("span");
            var text = new TextNode("hello");
            div.AppendChild(span);
            span.AppendChild(text);
            var content = div.GetContentNode();
            Assert.True(content is ContentElementNode);
            var ce = (ContentElementNode)content;
            Assert.Equal("div", ce.TagName);
            Assert.Single(ce.Children);
            Assert.Single(ce.Attributes);
            var att = ce.Attributes[0];
            Assert.Equal("id", att.Attribute);
            Assert.Equal("mydiv", att.Value);
        }
    }
}
