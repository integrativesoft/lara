/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        public LaraBuilder PushNS(string ns, string tagName)
        {
            var added = Element.CreateNS(ns, tagName);
            return Push(added);
        }

        public LaraBuilder Push(Element element)
        {
            var current = _stack.Peek();
            current.AppendChild(element);
            _stack.Push(element);
            return this;
        }

        public LaraBuilder Push(Element element, string className)
        {
            Push(element);
            element.Class = className;
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
            return AddNode(node);
        }

        public LaraBuilder AddNode(Node node)
        {
            var current = _stack.Peek();
            current.AppendChild(node);
            return this;
        }

        public LaraBuilder AddNodes(IEnumerable<Node> nodes)
        {
            var current = _stack.Peek();
            foreach (var node in nodes)
            {
                current.AppendChild(node);
            }
            return this;
        }

        public LaraBuilder AddNodes(IEnumerable<Element> nodes)
        {
            var current = _stack.Peek();
            foreach (var node in nodes)
            {
                current.AppendChild(node);
            }
            return this;
        }

        public LaraBuilder Add(Action<LaraBuilder> action)
        {
            action(this);
            return this;
        }

        public LaraBuilder Attribute(string attribute, string value)
        {
            var current = _stack.Peek();
            current.SetAttribute(attribute, value);
            return this;
        }

        public LaraBuilder FlagAttribute(string attribute, bool value)
        {
            var current = _stack.Peek();
            current.SetFlagAttribute(attribute, value);
            return this;
        }

        public LaraBuilder On(string eventName, Func<IPageContext, Task> handler)
        {
            _stack.Peek().On(eventName, handler);
            return this;
        }

        public LaraBuilder On(EventSettings settings)
        {
            _stack.Peek().On(settings);
            return this;
        }
    }
}
