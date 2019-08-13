/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Components;
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
            Register<Anchor>("a");
            Register<Button>("button");
            Register<ColGroup>("colgroup");
            Register<Image>("img");
            Register<Input>("input");
            Register<Label>("label");
            Register<Link>("link");
            Register<ListItem>("li");
            Register<Meta>("meta");
            Register<Meter>("meter");
            Register<Option>("option");
            Register<OptionGroup>("optgroup");
            Register<OrderedList>("ol");
            Register<Script>("script");
            Register<Select>("select");
            Register<Table>("table");
            Register<TableCell>("td");
            Register<TableHeader>("th");
            Register<TextArea>("textarea");
            Register<Slot>("slot");
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
            else if (tagName.IndexOf(' ') >= 0)
            {
                throw new ArgumentException("Element tag names cannot contain spaces.");
            }
            else
            {
                tagName = tagName.ToLowerInvariant();
            }
            if (FindTagName(tagName, out Type type))
            {
                return (Element)Activator.CreateInstance(type);
            }
            else
            {
                return new GenericElement(tagName);
            }
        }

        private static bool FindTagName(string tagName, out Type type)
        {
            return _map.TryGetValue(tagName, out type)
                || LaraUI.TryGetComponent(tagName, out type);
        }

        public static Element CreateElement(string tagName, string id)
        {
            var element = CreateElement(tagName);
            element.Id = id;
            return element;
        }

        public static Element CreateElementNS(string ns, string tagName)
        {
            var element = CreateElement(tagName);
            element.SetAttribute("xlmns", ns);
            return element;
        }
    }
}
