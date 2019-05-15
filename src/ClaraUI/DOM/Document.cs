/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Delta;
using Integrative.Clara.Main;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace Integrative.Clara.DOM
{
    public class Document
    {
        internal IPage Page { get; }
        internal Guid VirtualId { get; }
        private readonly DocumentIdMap _map;
        private readonly Queue<BaseDelta> _queue;
        internal SemaphoreSlim Semaphore { get; }

        int _serializer;

        public string Lang { get; set; }
        public Element Head { get; }
        public Element Body { get; }

        internal DateTime LastUTC { get; private set; }

        internal Document(IPage page, Guid virtualId)
        {
            VirtualId = virtualId;
            Page = page;
            _map = new DocumentIdMap();
            _queue = new Queue<BaseDelta>();
            Semaphore = new SemaphoreSlim(1);
            Head = new Element(this, "head");
            Body = new Element(this, "body");
            UpdateTimestamp();
        }

        public void UpdateTimestamp()
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

        public void OnElementAdded(Element element)
            => _map.NotifyAdded(element);

        public void OnElementRemoved(Element element)
            => _map.NotifyRemoved(element);

        public void NotifyChangeId(Element element, string before, string after)
            => _map.NotifyChangeId(element, before, after);

        internal bool QueueOpen { get; private set; }

        internal void OpenEventQueue() => QueueOpen = true;

        internal void CloseEventQueue() => QueueOpen = false;

        internal void Enqueue(BaseDelta delta)
        {
            _queue.Enqueue(delta);
        }

        public bool TryGetElementById(string id, out Element element)
            => _map.TryGetElementById(id, out element);

        public string FlushQueue()
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
    }
}
