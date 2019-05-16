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

namespace Integrative.Clara.Tests.Delta
{
    public class AttributeEditTesting
    {
        [Fact]
        public void AttributeEdited()
        {
            var doc = CreateDocument();
            var div = new Element("div", "mydiv");
            doc.Body.AppendChild(div);
            doc.OpenEventQueue();
            div.SetAttribute("data-test", "x");
            var queue = doc.GetQueue();
            Assert.Single(queue);
            var step = queue.Peek() as AttributeEditedDelta;
            Assert.Equal("data-test", step.Attribute);
            Assert.Equal("x", step.Value);
            Assert.Equal("mydiv", step.ElementId);
        }

        [Fact]
        public void IdRemoved()
        {
            var doc = CreateDocument();
            var div = new Element("div", "mydiv");
            doc.Body.AppendChild(div);
            doc.OpenEventQueue();
            div.Id = null;
            var queue = doc.GetQueue();
            Assert.Single(queue);
            var step = queue.Peek() as AttributeRemovedDelta;
            Assert.Equal("id", step.Attribute);
            Assert.Equal("mydiv", step.ElementId);
        }

        [Fact]
        public void UnchangedIdNoSteps()
        {
            var doc = CreateDocument();
            var div = new Element("div", "mydiv");
            doc.Body.AppendChild(div);
            doc.OpenEventQueue();
            div.Id = "mydiv";
            var queue = doc.GetQueue();
            Assert.Empty(queue);
        }

        [Fact]
        public void SetValueDeltaProperties()
        {
            var div = new Element("div", "mydiv");
            var doc = CreateDocument();
            doc.Body.AppendChild(div);
            doc.OpenEventQueue();
            div.SetAttribute("value", "x");
            var queue = doc.GetQueue();
            Assert.NotEmpty(queue);
            var step = queue.Peek() as SetValueDelta;
            Assert.NotNull(step);
            Assert.Equal("mydiv", step.ElementId);
            Assert.Equal("x", step.Value);
        }

        [Fact]
        public void EditAttributeClearId()
        {
            var div = new Element("div", "mydiv");
            var doc = CreateDocument();
            doc.Body.AppendChild(div);
            doc.OpenEventQueue();
            div.SetAttribute("id", null);
            var queue = doc.GetQueue();
            Assert.NotEmpty(queue);
            var step = queue.Peek() as AttributeRemovedDelta;
            Assert.NotNull(step);
            Assert.Equal("mydiv", step.ElementId);
            Assert.Equal("id", step.Attribute);
        }

        private static Document CreateDocument()
        {
            var page = new MyPage();
            var doc = new Document(page, Connections.CreateCryptographicallySecureGuid());
            return doc;
        }
    }
}
