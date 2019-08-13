/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;

namespace Integrative.Lara.Components
{
    sealed class ComponentRegistry
    {
        public const string DashRequired = "Invalid tag name. It needs to have a '-' and cannot have spaces.";
        public const string MustInherit = "Component types must inherit from the Component class";

        private readonly Dictionary<string, Type> _components;

        public ComponentRegistry()
        {
            _components = new Dictionary<string, Type>();
        }

        public void Register(string name, Type type)
        {
            if (!IsValidTagName(name))
            {
                throw new ArgumentException(DashRequired);
            }
            else if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            else if (!type.IsSubclassOf(typeof(WebComponent)))
            {
                throw new InvalidOperationException(MustInherit);
            }
            else if (_components.TryGetValue(name, out var previous))
            {
                var message = $"Duplicate entries for tag '{name}'. The class '{previous.FullName}' already registers the tag name.";
                throw new InvalidOperationException(message);
            }
            else
            {
                _components.Add(name, type);
            }
        }

        public void Unregister(string tagName)
        {
            _components.Remove(tagName);
        }

        private bool IsValidTagName(string tagName)
        {
            return !string.IsNullOrEmpty(tagName)
                && !tagName.Contains(" ")
                && tagName.Contains("-");
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
