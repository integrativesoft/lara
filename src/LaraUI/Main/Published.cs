/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Components;
using System;
using System.Collections.Generic;

namespace Integrative.Lara.Main
{
    sealed class Published : IDisposable
    {
        readonly Dictionary<string, IPublishedItem> _published;
        readonly ComponentRegistry _components;
        
        public Connections Connections { get; }

        public Published()
        {
            _published = new Dictionary<string, IPublishedItem>();
            _components = new ComponentRegistry();
            Connections = new Connections();
        }

        public void ClearAll()
        {
            _published.Clear();
            _components.Clear();
            Connections.Clear();
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
                throw new ArgumentException(Resources.SpecifyMethodService);
            }
        }

        internal static void ValidateAddress(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(Resources.SpecifyAddressService);
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

        public void Publish(WebComponentOptions options)
        {
            _components.Register(options.ComponentTagName, options.ComponentType);
        }

        public void UnPublishWebComponent(string componentTagName)
        {
            _components.Unregister(componentTagName);
        }

        public bool TryGetComponent(string tagName, out Type type)
        {
            return _components.TryGetComponent(tagName, out type);
        }
    }
}
