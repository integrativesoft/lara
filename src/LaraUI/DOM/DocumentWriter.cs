/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Globalization;
using System.Text;
using System.Web;

namespace Integrative.Lara
{
    internal sealed class DocumentWriter
    {
        public const int MaxLevelDeep = 500;

        private readonly Document? _document;
        private readonly StringBuilder _builder;

        public DocumentWriter(Document document)
            : this()
        {
            _document = document;
        }

        public DocumentWriter()
        {
            _builder = new StringBuilder();
        }

        public void Print()
        {
            if (_document == null)
            {
                return;
            }
            _builder.AppendLine("<!doctype html>");
            _builder.AppendLine("<!-- Rendered with Lara Web Engine. https://laraui.com -->");
            _builder.Append("<html");
            if (!string.IsNullOrEmpty(_document.Lang))
            {
                var lang = HttpUtility.HtmlAttributeEncode(_document.Lang);
                _builder.Append($" lang=\"{lang}\"");
            }
            _builder.AppendLine(">");
            PrintElement(_document.Head, 1);
            PrintElement(_document.Body, 1);
            _builder.AppendLine("</html>");
        }

        public void PrintElement(Element element, int indent)
        {
            VerifyNestedLevel(indent);
            if (IsInlineElement(element))
            {
                Indent(indent);
                PrintInlineElement(element);
                _builder.AppendLine();
            }
            else
            {
                PrintRegularElement(element, indent);
            }
        }

        internal static void VerifyNestedLevel(int indent)
        {
            if (indent > MaxLevelDeep)
            {
                throw new InvalidOperationException($"Document exceeded maximum nesting level of {MaxLevelDeep.ToString(CultureInfo.CurrentCulture)}.");
            }
        }

        private static bool IsInlineElement(Element element)
        {
            var hasChildren = false;
            foreach (var child in element.GetLightChildren())
            {
                hasChildren = true;
                if (child is TextNode)
                {
                    return true;
                }
            }
            return !hasChildren;
        }

        private void PrintInlineElement(Element element)
        {
            PrintOpeningTag(element);
            if (!HtmlReference.IsSelfClosingTag(element.TagName))
            {
                PrintInlineChildNodes(element);
                PrintClosingTag(element, 0);
            }
        }

        private void Indent(int indent)
        {
            for (var index = 0; index < indent; index++)
            {
                _builder.Append('\t');
            }
        }

        private void PrintOpeningTag(Element element)
        {
            _builder.Append('<');
            _builder.Append(element.TagName);
            PrintAttributes(element);
            _builder.Append('>');
        }

        private void PrintAttributes(Element element)
        {
            foreach (var pair in element.Attributes)
            {
                _builder.Append(' ');
                _builder.Append(pair.Key);
                var value = pair.Value;
                if (value != null)
                {
                    _builder.Append('=');
                    _builder.Append('"');
                    _builder.Append(HttpUtility.HtmlAttributeEncode(value));
                    _builder.Append('"');
                }
            }
        }

        private void PrintInlineChildNodes(Element element)
        {
            foreach (var child in element.GetLightChildren())
            {
                PrintInlineNode(child);
            }
        }

        private void PrintInlineNode(Node node)
        {
            if (node is TextNode text)
            {
                _builder.Append(text.Data);
            }
            else if (node is Element element)
            {
                PrintInlineElement(element);
            }
        }

        private void PrintRegularElement(Element element, int indent)
        {
            Indent(indent);
            PrintOpeningTag(element);
            _builder.AppendLine();
            if (!HtmlReference.IsSelfClosingTag(element.TagName))
            {
                PrintChildNodes(element, indent + 1);
                PrintClosingTag(element, indent);
            }
            _builder.AppendLine();
        }

        private void PrintChildNodes(Element element, int indent)
        {
            foreach (var child in element.GetLightChildren())
            {
                PrintElement((Element)child, indent);
            }
        }

        private void PrintClosingTag(Element element, int indent)
        {
            Indent(indent);
            _builder.Append('<');
            _builder.Append('/');
            _builder.Append(element.TagName);
            _builder.Append('>');
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
