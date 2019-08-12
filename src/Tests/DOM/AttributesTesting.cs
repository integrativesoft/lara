/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using Integrative.Lara.DOM;
using Integrative.Lara.Front.Tools;
using Integrative.Lara.Main;
using Integrative.Lara.Tests.Main;
using System;
using Xunit;

namespace Integrative.Lara.Tests.DOM
{
    public class AttributesTesting
    {
        [Fact]
        public void HasAttributeFindsAttribute()
        {
            var x = new Attributes(Element.Create("button"));
            x.SetAttributeLower("href", "lala");
            Assert.True(x.HasAttribute("href"));
            Assert.False(x.HasAttribute("lala"));
            Assert.Equal("lala", x.GetAttribute("HREF"));
        }

        [Fact]
        public void ValueAttributeEnqueued()
        {
            var document = CreateDocument();
            var element = Element.Create("button", "mybutton");
            document.Body.AppendChild(element);
            document.OpenEventQueue();
            element.SetAttribute("value", "5");
            var queue = document.GetQueue();
            Assert.NotEmpty(queue);
            var peek = queue.Peek();
            Assert.True(peek is SetValueDelta);
        }

        private Document CreateDocument()
        {
            var guid = Guid.Parse("{0857AE93-8591-4CB6-887E-C449ABFCAA7A}");
            var page = new MyPage();
            return new Document(page, guid, new LaraOptions());
        }

        [Fact]
        public void SetFlagAttributeAddsNullValue()
        {
            var element = Element.Create("span");
            element.Hidden = true;
            Assert.True(element.HasAttribute("hidden"));
            Assert.Null(element.GetAttribute("hidden"));
            int count = 0;
            foreach (var pair in element.Attributes)
            {
                count++;
                Assert.Equal("hidden", pair.Key);
                Assert.Null(pair.Value);
            }
            Assert.Equal(1, count);
            element.Hidden = false;
            Assert.False(element.HasAttribute("hidden"));
        }

        [Fact]
        public void GetNonExistingReturnsEmpty()
        {
            var element = Element.Create("button");
            Assert.Equal(string.Empty, element.GetAttribute("lele"));
        }

        [Fact]
        public void RemovingAttributeRemovesValue()
        {
            var element = Element.Create("button");
            element.SetAttribute("data-test", "lala");
            element.RemoveAttribute("data-test");
            Assert.Empty(element.GetAttribute("data-test"));
        }

        [Fact]
        public void ReplacingSameValueNoQueue()
        {
            var element = Element.Create("div");
            element.Class = "lala";
            var document = new Document(new MyPage());
            document.Body.AppendChild(element);
            document.OpenEventQueue();
            element.Class = "lala";
            Assert.Empty(document.GetQueue());
        }

        [Fact]
        public void NotifySelectedSetsSelected()
        {
            var option = new Option();
            option.NotifyValue(new ElementEventValue
            {
                Checked = true,
            });
            Assert.True(option.Selected);
        }

        [Fact]
        public void CreateNsSetsXlmns()
        {
            var x = Element.CreateNS("abc", "svg");
            Assert.Equal("abc", x.GetAttribute("xlmns"));
        }

        [Fact]
        public void RemoveAttributeMissingSucceeds()
        {
            var document = CreateDocument();
            var x = Element.Create("div");
            document.Body.AppendChild(x);
            document.OpenEventQueue();
            x.RemoveAttribute("lala");
            Assert.Empty(document.GetQueue());            
        }

        [Fact]
        public void AttributesNotifyValueRemovesPrevious()
        {
            var x = Element.Create("div");
            x.SetAttributeLower("value", "one");
            x.NotifyValue("two");
            Assert.Equal("two", x.GetAttribute("value"));
        }

        [Fact]
        public void MaxLevelDeep()
        {
            bool found = false;
            try
            {
                DocumentWriter.VerifyNestedLevel(DocumentWriter.MaxLevelDeep + 1);
            }
            catch (InvalidOperationException)
            {
                found = true;
            }
            Assert.True(found);
        }

        [Fact]
        public void ToggleClassFlipsClass()
        {
            Assert.Equal("lala", ClassEditor.ToggleClass("lala lolo", "lolo"));
            Assert.Equal("lala lolo", ClassEditor.ToggleClass("lala", "lolo"));
        }
    }
}
