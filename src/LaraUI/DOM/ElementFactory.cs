/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

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
                return new GenericElement(tagName);
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
