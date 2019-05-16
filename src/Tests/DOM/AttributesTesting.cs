/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Delta;
using Integrative.Clara.DOM;
using Integrative.Clara.Main;
using Integrative.Clara.Tests.Main;
using Xunit;

namespace Integrative.Clara.Tests.DOM
{
    public class AttributesTesting
    {
        [Fact]
        public void HasAttributeFindsAttribute()
        {
            var x = new Attributes(new Element("button"));
            x.SetAttributeLower("href", "lala");
            Assert.True(x.HasAttribute("href"));
            Assert.False(x.HasAttribute("lala"));
            Assert.Equal("lala", x.GetAttribute("HREF"));
        }

        [Fact]
        public void ValueAttributeEnqueued()
        {
            var guid = Connections.CreateCryptographicallySecureGuid();
            var page = new MyPage();
            var document = new Document(page, guid);
            var element = new Element("button")
            {
                Id = "mybutton"
            };
            document.Body.AppendChild(element);
            document.OpenEventQueue();
            element.SetAttribute("value", "5");
            var queue = document.GetQueue();
            Assert.NotEmpty(queue);
            var peek = queue.Peek();
            Assert.True(peek is SetValueDelta);
        }

        [Fact]
        public void SetFlagAttributeAddsNullValue()
        {
            var element = new Element("button")
            {
                Hidden = true
            };
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
            var element = new Element("button");
            Assert.Equal(string.Empty, element.GetAttribute("lele"));
        }

        [Fact]
        public void RemovingAttributeRemovesValue()
        {
            var element = new Element("button");
            element.SetAttribute("data-test", "lala");
            element.RemoveAttribute("data-test");
            Assert.Empty(element.GetAttribute("data-test"));
        }

    }
}
