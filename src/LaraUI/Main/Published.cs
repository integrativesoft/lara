/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;

namespace Integrative.Lara.Main
{
    sealed class Published : IDisposable
    {
        readonly Dictionary<string, IPublishedItem> _published;
        
        public Connections Connections { get; }

        public Published()
        {
            _published = new Dictionary<string, IPublishedItem>();
            Connections = new Connections();
        }

        public void Publish(string path, IPublishedItem item)
        {
            _published.Remove(path);
            _published.Add(path, item);
        }

        public void UnPublish(string path)
        {
            _published.Remove(path);
        }

        public bool TryGetNode(string path, out IPublishedItem item)
        {
            return _published.TryGetValue(path, out item);
        }

        public void ClearAll()
        {
            _published.Clear();
            Connections.ClearAll();
        }

        bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _published.Clear();
                Connections.Dispose();
            }
        }
    }
}
