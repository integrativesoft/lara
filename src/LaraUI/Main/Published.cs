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

        public void Publish(WebServiceContent content)
        {
            ValidateAddress(content.Address);
            ValidateMethod(content.Method);
            var combined = CombinePathMethod(content.Address, content.Method.ToUpperInvariant());
            _published.Remove(combined);
            _published.Add(combined, new WebServicePublished(content));
        }

        public static string CombinePathMethod(string path, string method)
        {
            if (method == "GET")
            {
                return path;
            }
            else
            {
                return path + ">" + method;
            }
        }

        internal static void ValidateMethod(string method)
        {
            if (string.IsNullOrEmpty(method))
            {
                throw new ArgumentException("Please specify the method for the web service (i.e. 'POST').");
            }
        }

        internal static void ValidateAddress(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Please specify the address for the web service.");
            }
        }

        public void UnPublish(string path)
        {
            _published.Remove(path);
        }

        public void UnPublish(string path, string method)
        {
            var combined = CombinePathMethod(path, method.ToUpperInvariant());
            _published.Remove(combined);
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
