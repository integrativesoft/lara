/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;

namespace Integrative.Lara.Main
{
    sealed class SessionStorage
    {
        readonly Dictionary<string, string> _values;
        readonly object _mylock;

        public SessionStorage()
        {
            _values = new Dictionary<string, string>();
            _mylock = new object();
        }

        public void Save(string key, string value)
        {
            lock (_mylock)
            {
                _values.Remove(key);
                _values.Add(key, value);
            }
        }

        public void Remove(string key)
        {
            lock (_mylock)
            {
                _values.Remove(key);
            }
        }

        public bool TryGetValue(string key, out string value)
        {
            lock (_mylock)
            {
                return _values.TryGetValue(key, out value);
            }
        }
    }
}
