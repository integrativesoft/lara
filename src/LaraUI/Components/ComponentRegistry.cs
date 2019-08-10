/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Integrative.Lara.Components
{
    static class ComponentRegistry
    {
        private readonly static Dictionary<string, Type> _components;

        static ComponentRegistry()
        {
            _components = new Dictionary<string, Type>();
        }

        public static void Register(string name, Type type)
        {
            if (!IsValidTagName(name))
            {
                throw new ArgumentNullException("Invalid tag name. It needs to have a '-' and cannot have spaces.");
            }
            else if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            else if (!type.IsSubclassOf(typeof(Component)))
            {
                throw new InvalidOperationException("Component types must inherit from the Component class");
            }
            else if (_components.TryGetValue(name, out var previous))
            {
                var message = $"Duplicate entries for tag '{name}'. The class '{previous.FullName}' already registers the tag name.";
                throw new InvalidOperationException(message);
            }

        }

        private static bool IsValidTagName(string tagName)
        {
            return !string.IsNullOrEmpty(tagName)
                && !tagName.Contains(" ")
                && tagName.Contains("-");
        }

        public static bool TryGetComponent(string name, out Type type)
        {
            return _components.TryGetValue(name, out type);
        }
    }
}
