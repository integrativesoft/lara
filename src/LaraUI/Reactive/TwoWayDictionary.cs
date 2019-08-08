/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/
/*
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Integrative.Lara.Reactive
{
    class TwoWayDictionary<TKey, TChild> : IEnumerable<KeyValuePair<TKey, TChild>>
    {
        readonly Dictionary<TKey, TChild> _values;
        readonly Dictionary<TChild, HashSet<TKey>> _reverse;

        public TwoWayDictionary()
        {
            _values = new Dictionary<TKey, TChild>();
            _reverse = new Dictionary<TChild, HashSet<TKey>>();
        }

        public void Add(TKey key, TChild value)
        {
            _values.Add(key, value);
            if (!_reverse.TryGetValue(value, out var set))
            {
                set = new HashSet<TKey>();
                _reverse.Add(value, set);
            }
            set.Add(key);
        }

        public bool TryGetValue(TKey key, out TChild value)
        {
            return _values.TryGetValue(key, out value);
        }

        public void Remove(TKey key)
        {
            if (_values.TryGetValue(key, out var value))
            {
                _values.Remove(key);
                if (_reverse.TryGetValue(value, out var set))
                {
                    set.Remove(key);
                    if (set.Count == 0)
                    {
                        _reverse.Remove(value);
                    }
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TChild>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        public IEnumerable<TKey> Keys => _values.Keys;

        public IEnumerable<TChild> Values => _values.Values;

        public IEnumerable<TKey> GetChildKeys(TChild child)
        {
            if (_reverse.TryGetValue(child, out var set))
            {
                return set;
            }
            else
            {
                return Enumerable.Empty<TKey>();
            }
        }
    }
}
*/