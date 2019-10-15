/*
Copyright (c) 2019 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using Integrative.Lara.Tests.DOM;
using Xunit;

namespace Integrative.Lara.Tests.Delta
{
    public class DeltaTesting
    {
        [Fact]
        public void SubmitJsStores()
        {
            var x = new SubmitJsDelta
            {
                Code = "code"
            };
            Assert.Equal("code", x.Code);
        }

        [Fact]
        public void ReplaceDeltaEnqueues()
        {
            var doc = DomOperationsTesting.CreateDocument();
            doc.OpenEventQueue();
            ReplaceDelta.Enqueue(doc, "test");
            var q = doc.GetQueue();
            Assert.Single(q);
            var first = q.Peek() as ReplaceDelta;
            Assert.NotNull(first);
            Assert.Equal("test", first.Location);
        }

        [Fact]
        public void TextModifiedGenerated()
        {
            var doc = DomOperationsTesting.CreateDocument();
            var span = Element.Create("span");
            span.SetInnerText("a");
            doc.Body.AppendChild(span);
            doc.OpenEventQueue();
            span.SetInnerText("test");
            var q = doc.GetQueue();
            Assert.Single(q);
            var first = q.Peek() as TextModifiedDelta;
            Assert.NotNull(first);
            Assert.Equal("test", first.Text);
            Assert.Equal(span.EnsureElementId(), first.ParentElementId);
            Assert.Equal(0, first.ChildNodeIndex);
        }

        [Fact]
        public void ElementEventValueData()
        {
            var x = new ElementEventValue
            {
                Checked = true,
                ElementId = "a",
                Value = "text"
            };
            Assert.Equal("#a='text' checked", x.ToString());
            x.Checked = false;
            Assert.Equal("#a='text'", x.ToString());
        }

        [Fact]
        public void PlugOptionsLongRunning()
        {
            var x = new PlugOptions
            {
                LongRunning = true,
                Block = true
            };
            Assert.True(x.LongRunning);
            Assert.True(x.Block);
        }

        [Fact]
        public void PlugOptionsSerialize()
        {
            var x = new PlugOptions
            {
                Block = true
            };
            var json1 = x.ToJSON();
            var json2 = LaraUI.JSON.Stringify(x);
            Assert.Equal(json1, json2);
        }

        [Fact]
        public void ClientEventFromSettings()
        {
            var x = new EventSettings
            {
                Block = true,
                BlockOptions = new BlockOptions
                {
                    BlockedElementId = "a",
                    ShowElementId = "b",
                    ShowHtmlMessage = "c"
                },
                EventName = "click",
                LongRunning = true
            };
            var client = ClientEventSettings.CreateFrom(x);
            client.ExtraData = "xx";
            Assert.Equal(x.Block, client.Block);
            Assert.Equal(x.BlockOptions.ShowElementId, client.BlockShownId);
            Assert.Equal(x.BlockOptions.BlockedElementId, client.BlockElementId);
            Assert.Equal(x.BlockOptions.ShowHtmlMessage, client.BlockHTML);
            Assert.Equal(x.EventName, client.EventName);
            Assert.Equal(x.LongRunning, client.LongRunning);
            Assert.Equal("xx", client.ExtraData);
        }
    }
}
