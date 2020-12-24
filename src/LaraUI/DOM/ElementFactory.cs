/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Integrative.Lara
{
    internal static class ElementFactory
    {
        private static readonly Dictionary<string, Type> _Map;

        static ElementFactory()
        {
            _Map = new Dictionary<string, Type>();
            Register<HtmlAnchorElement>("a");
            Register<HtmlBodyElement>("body");
            Register<HtmlButtonElement>("button");
            Register<HtmlColGroupElement>("colgroup");
            Register<HtmlHeadElement>("head");
            Register<HtmlImageElement>("img");
            Register<HtmlInputElement>("input");
            Register<HtmlLabelElement>("label");
            Register<HtmlLinkElement>("link");
            Register<HtmlLiElement>("li");
            Register<HtmlMetaElement>("meta");
            Register<HtmlMeterElement>("meter");
            Register<HtmlOptionElement>("option");
            Register<HtmlOptionGroupElement>("optgroup");
            Register<HtmlOlElement>("ol");
            Register<HtmlScriptElement>("script");
            Register<HtmlSelectElement>("select");
            Register<HtmlTableElement>("table");
            Register<HtmlTableCellElement>("td");
            Register<HtmlTableHeaderElement>("th");
            Register<HtmlTextAreaElement>("textarea");
            Register<Slot>("slot");
            Register<HtmlSpanElement>("span");
            Register<HtmlDivElement>("div");
        }

        private static void Register<T>(string lowerTagName) where T : Element
        {
            _Map.Add(lowerTagName, typeof(T));
        }

        public static Element CreateElement(string tagName)
        {
            tagName = VerifyTagName(tagName);

            if (FindTagName(tagName, out var type))
            {
                return (Element)Activator.CreateInstance(type);
            }

            return new GenericElement(tagName);
        }

        private static string VerifyTagName(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
            {
                throw new ArgumentException(Resources.InvalidTagName);
            }

            if (tagName.Contains(' ', StringComparison.InvariantCulture))
            {
                throw new ArgumentException(Resources.TagNameSpaces);
            }

            return tagName.ToLowerInvariant();
        }

        private static bool FindTagName(string tagName, [NotNullWhen(true)] out Type? type)
        {
            return _Map.TryGetValue(tagName, out type)
                || LaraUI.Context.Application.TryGetComponent(tagName, out type);
        }

        public static Element CreateElement(string tagName, string id)
        {
            var element = CreateElement(tagName);
            element.Id = id;
            return element;
        }

        public static Element CreateElementNs(string ns, string tagName)
        {
            var element = CreateElement(tagName);
            element.SetAttribute("xlmns", ns);
            return element;
        }
    }
}
