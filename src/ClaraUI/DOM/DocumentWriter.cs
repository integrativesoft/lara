/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Text;
using System.Web;

namespace Integrative.Clara.DOM
{
    sealed class DocumentWriter
    {
        readonly Document _document;
        readonly StringBuilder _builder;

        public DocumentWriter(Document document)
        {
            _document = document;
            _builder = new StringBuilder();
        }

        public void Print()
        {
            _builder.AppendLine("<!doctype html>");
            _builder.AppendLine("<!-- Rendered with ClaraUI. https://claraui.com -->");
            _builder.Append("<html");
            if (!string.IsNullOrEmpty(_document.Lang))
            {
                string lang = HttpUtility.HtmlAttributeEncode(_document.Lang);
                _builder.Append($" lang=\"{lang}\"");
            }
            _builder.AppendLine(">");
            PrintElement(_document.Head, 1);
            PrintElement(_document.Body, 1);
            _builder.AppendLine("</html>");
        }

        public void PrintElement(Element element, int indent)
        {
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

        private bool IsInlineElement(Element element)
        {
            bool hasChildren = false;
            foreach (var child in element.Children)
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
            for (int index = 0; index < indent; index++)
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
                    _builder.Append(value);
                    _builder.Append('"');
                }
            }
        }

        private void PrintInlineChildNodes(Element element)
        {
            foreach (var child in element.Children)
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
            foreach (var child in element.Children)
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
