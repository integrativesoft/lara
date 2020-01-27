/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;

namespace Integrative.Lara
{
    internal sealed class DocumentIdMap
    {
        private readonly Dictionary<string, Element> _map;

        public DocumentIdMap()
        {
            _map = new Dictionary<string, Element>();
        }

        public bool TryGetElementById(string id, out Element element)
        {
            return _map.TryGetValue(id, out element);
        }

        public void NotifyChangeId(Element element, string? before, string? after)
        {
            if (before != after)
            {
                if (!string.IsNullOrEmpty(before))
                {
                    RemovePrevious(before);
                }
                if (!string.IsNullOrEmpty(after))
                {
                    AddAfter(element, after);
                }
            }
        }

        private void RemovePrevious(string before)
        {
            _map.Remove(before);
        }

        private void AddAfter(Element element, string after)
        {
            if (_map.ContainsKey(after))
            {
                throw DuplicateElementIdException.Create(after);
            }
            _map.Add(after, element);
        }

        public void NotifyRemoved(Element element)
        {
            if (element.Id != null)
            {
                RemovePrevious(element.Id);
            }
        }

        public void NotifyAdded(Element element)
        {
            if (element.Id != null)
            {
                AddAfter(element, element.Id);
            }
        }
    }
}
