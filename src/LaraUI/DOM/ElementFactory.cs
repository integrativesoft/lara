/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;

namespace Integrative.Lara
{
    internal static class ElementFactory
    {
        private readonly static Dictionary<string, Func<Element>> _Creators
            = new Dictionary<string, Func<Element>>
            {
                { "a", () => new HtmlAnchorElement() },
                { "body", () => new HtmlBodyElement() },
                { "button", () => new HtmlButtonElement() },
                { "colgroup", () => new HtmlColGroupElement() },
                { "div", () => new HtmlDivElement() },
                { "h1", () => new HtmlHeadingElement(1) },
                { "h2", () => new HtmlHeadingElement(2) },
                { "h3", () => new HtmlHeadingElement(3) },
                { "h4", () => new HtmlHeadingElement(4) },
                { "h5", () => new HtmlHeadingElement(5) },
                { "h6", () => new HtmlHeadingElement(6) },
                { "head", () => new HtmlHeadElement() },
                { "img", () => new HtmlImageElement() },
                { "input", () => new HtmlInputElement() },
                { "label", () => new HtmlLabelElement() },
                { "link", () => new HtmlLinkElement() },
                { "li", () => new HtmlLiElement() },
                { "meta", () => new HtmlMetaElement() },
                { "meter", () => new HtmlMeterElement() },
                { "option", () => new HtmlOptionElement() },
                { "optgroup", () => new HtmlOptionGroupElement() },
                { "ol", () => new HtmlOlElement() },
                { "p", () => new HtmlParagraphElement() },
                { "script", () => new HtmlScriptElement() },
                { "select", () => new HtmlSelectElement() },
                { "slot", () => new Slot() },
                { "span", () => new HtmlSpanElement() },
                { "table", () => new HtmlTableElement() },
                { "tbody", () => new HtmlTableSectionElement(HtmlTableSectionType.Body) },
                { "thead", () => new HtmlTableSectionElement(HtmlTableSectionType.Head) },
                { "tfoot", () => new HtmlTableSectionElement(HtmlTableSectionType.Foot) },
                { "td", () => new HtmlTableCellElement() },
                { "th", () => new HtmlTableHeaderElement() },
            };

        public static Element CreateElement(string tagName)
        {
            tagName = VerifyTagName(tagName);

            if (_Creators.TryGetValue(tagName, out var creator))
            {
                return creator();
            }
            if (LaraUI.Context.Application.TryGetComponent(tagName, out var type))
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
