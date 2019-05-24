/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Front.Elements;
using System;
using System.Collections.Generic;

namespace Integrative.Lara.DOM
{
    static class ElementFactory
    {
        private static readonly Dictionary<string, Type> _map;

        static ElementFactory()
        {
            _map = new Dictionary<string, Type>();
            Register<Button>("button");
        }

        private static void Register<T>(string lowerTagName) where T : Element
        {
            _map.Add(lowerTagName, typeof(T));
        }

        public static Element CreateElement(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                throw new ArgumentNullException(nameof(tagName));
            }
            else
            {
                tagName = tagName.ToLowerInvariant();
            }
            if (_map.TryGetValue(tagName, out var type))
            {
                return (Element)Activator.CreateInstance(type);
            }
            else
            {
                return new Element(tagName);
            }
        }

        public static Element CreateElement(string tagName, string id)
        {
            var element = CreateElement(tagName);
            element.Id = id;
            return element;
        }
    }
}
