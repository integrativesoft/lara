/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Integrative.Lara
{
    internal class AutocompleteRegistry
    {
        private readonly SessionLocal<Dictionary<string, AutocompleteElement>> _map;
        private readonly object _mapLock = new object();

        public AutocompleteRegistry()
        {
            _map = new SessionLocal<Dictionary<string, AutocompleteElement>>();
        }

        public bool TryGet(string key, [NotNullWhen(true)] out AutocompleteElement? element)
        {
            element = default;
            lock (_mapLock)
            {
                var map = _map.Value;
                return map != null
                    && map.TryGetValue(key, out element);
            }
        }

        public void Set(string key, AutocompleteElement element)
        {
            lock (_mapLock)
            {
                var map = _map.Value;
                if (map == null)
                {
                    map = new Dictionary<string, AutocompleteElement>();
                    _map.Value = map;
                    map.Add(key, element);
                }
                else
                {
                    map.Remove(key);
                    map.Add(key, element);
                }
            }
        }

        public void Remove(string key)
        {
            lock (_mapLock)
            {
                _map.Value?.Remove(key);
            }
        }

        public int Count => GetCount();

        private int GetCount()
        {
            var value = GetValue();
            return value?.Count ?? 0;
        }

        private Dictionary<string, AutocompleteElement>? GetValue()
        {
            lock (_mapLock)
            {
                return _map.Value;
            }
        }
    }
}
