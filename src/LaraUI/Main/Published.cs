/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;

namespace Integrative.Lara
{
    internal sealed class Published : IDisposable
    {
        private readonly Dictionary<string, IPublishedItem> _published;
        private readonly ComponentRegistry _components;
        
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

        private bool _disposed;

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
            var combined = CombineAddress(content.Address, content.Method);
            _published.Remove(combined);
            _published.Add(combined, new WebServicePublished(content));
        }

        public void Publish(BinaryServiceContent content)
        {
            var combined = CombineAddress(content.Address, content.Method);
            _published.Remove(combined);
            _published.Add(combined, new BinaryServicePublished(content));
        }

        private static string CombineAddress(string address, string method)
        {
            ValidateAddress(address);
            ValidateMethod(method);
            return CombinePathMethod(address, method.ToUpperInvariant());
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

        internal static void ValidateMethod(string? method)
        {
            if (string.IsNullOrEmpty(method))
            {
                throw new ArgumentException(Resources.SpecifyMethodService);
            }
        }

        internal static void ValidateAddress(string? path)
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
            _components.Register(options.ComponentTagName, options.GetComponentType());
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
