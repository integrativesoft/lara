/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using Integrative.Lara.DOM;
using Integrative.Lara.Main;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// An HTML5 document.
    /// </summary>
    public class Document
    {
        internal IPage Page { get; }
        internal Guid VirtualId { get; }
        private readonly DocumentIdMap _map;
        private readonly Queue<BaseDelta> _queue;
        internal SemaphoreSlim Semaphore { get; }

        int _serializer;

        /// <summary>
        /// Gets or sets the language. See 'lang' property for HTML5 documents.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Lang { get; set; }

        /// <summary>
        /// The document's Head element.
        /// </summary>
        /// <value>
        /// The head.
        /// </value>
        public Element Head { get; }

        /// <summary>
        /// The document's Body element.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public Element Body { get; }

        internal DateTime LastUTC { get; private set; }

        internal Queue<BaseDelta> GetQueue() => _queue;

        internal Document(IPage page)
            : this(page, new LaraOptions())
        {
        }

        internal Document(IPage page, LaraOptions options)
            : this(page, Connections.CreateCryptographicallySecureGuid(), options)
        {
        }

        internal Document(IPage page, Guid virtualId, LaraOptions options)
        {
            VirtualId = virtualId;
            Page = page;
            _map = new DocumentIdMap();
            _queue = new Queue<BaseDelta>();
            Semaphore = new SemaphoreSlim(1);
            Head = Element.Create("head");
            Head.Document = this;
            Body = Element.Create("body");
            Body.Document = this;
            UpdateTimestamp();
            TemplateBuilder.Build(this, options);
        }

        /// <summary>
        /// Creates an HTML element.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>Element created</returns>
        public Element CreateElement(string tagName) => Element.Create(tagName);

        /// <summary>
        /// Creates a text node.
        /// </summary>
        /// <param name="data">The node's data.</param>
        /// <returns>Text node created</returns>
        public TextNode CreateTextNode(string data) => new TextNode(data);

        internal void UpdateTimestamp()
        {
            LastUTC = DateTime.UtcNow;
        }

        internal void ModifyLastUtcForTesting(DateTime value)
        {
            LastUTC = value;
        }

        internal string GenerateElementId()
        {
            _serializer++;
            return "_e" + _serializer.ToString(CultureInfo.InvariantCulture);
        }

        internal void OnElementAdded(Element element)
            => _map.NotifyAdded(element);

        internal void OnElementRemoved(Element element)
            => _map.NotifyRemoved(element);

        internal void NotifyChangeId(Element element, string before, string after)
            => _map.NotifyChangeId(element, before, after);

        internal bool QueueOpen { get; private set; }

        internal void OpenEventQueue() => QueueOpen = true;

        internal void Enqueue(BaseDelta delta)
        {
            _queue.Enqueue(delta);
        }

        /// <summary>
        /// Retrieves the element with the given ID.
        /// </summary>
        /// <param name="id">Element ID</param>
        /// <param name="element">The element.</param>
        /// <returns>True when the element was found.</returns>
        public bool TryGetElementById(string id, out Element element)
            => _map.TryGetElementById(id, out element);

        /// <summary>
        /// Retrieves the element with the given ID.
        /// </summary>
        /// <param name="id">Element ID</param>
        /// <returns>The element</returns>
        public Element GetElementById(string id)
        {
            _map.TryGetElementById(id, out var element);
            return element;
        }

        internal string FlushQueue()
        {
            var list = new List<BaseDelta>();
            while (_queue.Count > 0)
            {
                var step = _queue.Dequeue();
                list.Add(step);
            }
            var result = new EventResult(list);
            return result.ToJSON();
        }

        internal void OnMessage(string key, Func<IPageContext, Task> handler)
        {
            Head.On("_" + key, handler);
        }
    }
}
