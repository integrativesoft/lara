/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;

namespace Integrative.Lara
{
    internal sealed class ComponentRegistry
    {
        private readonly Dictionary<string, Type> _components;

        public ComponentRegistry()
        {
            _components = new Dictionary<string, Type>();
        }

        public void Register(string name, Type type)
        {
            name = name ?? throw new ArgumentNullException(nameof(name));
            type = type ?? throw new ArgumentNullException(nameof(type));
            if (!IsValidTagName(name))
            {
                throw new ArgumentException(Resources.DashRequired);
            }

            if (!type.IsSubclassOf(typeof(WebComponent)))
            {
                throw new InvalidOperationException(Resources.MustInherit);
            }
            if (_components.TryGetValue(name, out var previous))
            {
                var message = $"Duplicate entries for tag '{name}'. The class '{previous.FullName}' already registers the tag name.";
                throw new InvalidOperationException(message);
            }
            _components.Add(name, type);
        }

        public void Unregister(string tagName)
        {
            _components.Remove(tagName);
        }

        private static bool IsValidTagName(string tagName)
        {
            return !string.IsNullOrEmpty(tagName)
                && !tagName.Contains(" ", StringComparison.InvariantCulture)
                && tagName.Contains("-", StringComparison.InvariantCulture);
        }

        public bool TryGetComponent(string name, out Type type)
        {
            return _components.TryGetValue(name, out type);
        }

        public void Clear()
        {
            _components.Clear();
        }
    }
}
