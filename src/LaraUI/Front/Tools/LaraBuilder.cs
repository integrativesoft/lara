/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;

namespace Integrative.Lara
{
    public sealed class LaraBuilder
    {
        readonly Stack<Element> _stack;

        public LaraBuilder(Element startingElement)
        {
            _stack = new Stack<Element>();
            _stack.Push(startingElement);
        }

        public LaraBuilder Push(string tagName, string className = null, string id = null)
        {
            return Push(tagName, className, id, out _);
        }

        public LaraBuilder Push(string tagName, string className, string id, out Element added)
        {
            added = Element.Create(tagName);
            if (!string.IsNullOrEmpty(id))
            {
                added.Id = id;
            }
            if (!string.IsNullOrEmpty(className))
            {
                added.Class = className;
            }
            return Push(added);
        }

        public LaraBuilder Push(Element element)
        {
            var current = _stack.Peek();
            current.AppendChild(element);
            _stack.Push(element);
            return this;
        }

        public LaraBuilder Pop()
        {
            if (_stack.Count <= 1)
            {
                throw new InvalidOperationException("Too many Pop() calls.");
            }
            _stack.Pop();
            return this;
        }

        public LaraBuilder AddTextNode(string text, bool encode = true)
        {
            return AddTextNode(new TextNode(text, encode));
        }

        public LaraBuilder AddTextNode(TextNode node)
        {
            var current = _stack.Peek();
            current.AppendChild(node);
            return this;
        }

        public LaraBuilder Attribute(string attribute, string value)
        {
            var current = _stack.Peek();
            current.SetAttribute(attribute, value);
            return this;
        }
    }
}
