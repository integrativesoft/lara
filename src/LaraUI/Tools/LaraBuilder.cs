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
    /// <summary>
    /// Class to build pages more easily
    /// </summary>
    public sealed class LaraBuilder
    {
        readonly Stack<Element> _stack;

        /// <summary>
        /// Initializes a new instance of the <see cref="LaraBuilder"/> class.
        /// </summary>
        /// <param name="startingElement">The starting element to build on.</param>
        public LaraBuilder(Element startingElement)
        {
            _stack = new Stack<Element>();
            startingElement.EnsureElementId();
            _stack.Push(startingElement);
        }

        /// <summary>
        /// Adds a new element and positions the builder into it.
        /// </summary>
        /// <param name="tagName">Element's tag name.</param>
        /// <param name="className">Element's class name</param>
        /// <param name="id">element's identifier.</param>
        /// <returns>This instance</returns>
        public LaraBuilder Push(string tagName, string className = null, string id = null)
        {
            return Push(tagName, className, id, out _);
        }

        /// <summary>
        /// Adds a new element and positions the builder into it.
        /// </summary>
        /// <param name="tagName">Element's tag name.</param>
        /// <param name="className">Element's class name</param>
        /// <param name="id">The identifier.</param>
        /// <param name="added">The element added.</param>
        /// <returns>This instance</returns>
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

        /// <summary>
        /// Adds a new namespace-specific element (e.g. 'svg') and positions the builder into it.
        /// </summary>
        /// <param name="ns">The namespace of the element.</param>
        /// <param name="tagName">The element's tag name</param>
        /// <returns>This instance</returns>
        public LaraBuilder PushNS(string ns, string tagName)
        {
            var added = Element.CreateNS(ns, tagName);
            return Push(added);
        }

        /// <summary>
        /// Adds an element and positions the builder into it.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>This instance</returns>
        public LaraBuilder Push(Element element)
        {
            element = element ?? throw new ArgumentNullException(nameof(element));
            element.EnsureElementId();
            var current = _stack.Peek();
            current.AppendChild(element);
            _stack.Push(element);
            return this;
        }

        /// <summary>
        /// Adds an element and positions the builder into it.
        /// </summary>
        /// <param name="element">The element to add.</param>
        /// <param name="className">Name of the class.</param>
        /// <returns>This instance</returns>
        public LaraBuilder Push(Element element, string className)
        {
            Push(element);
            element.Class = className;
            return this;
        }

        /// <summary>
        /// Positions the builder one position back on the element stack.
        /// </summary>
        /// <returns>This instance</returns>
        /// <exception cref="InvalidOperationException">Too many Pop() calls.</exception>
        public LaraBuilder Pop()
        {
            if (_stack.Count <= 1)
            {
                throw new InvalidOperationException("Too many Pop() calls.");
            }
            _stack.Pop();
            return this;
        }

        /// <summary>
        /// Adds a text node.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="encode">if set to <c>true</c> [encode].</param>
        /// <returns>This instance</returns>
        public LaraBuilder AddTextNode(string text, bool encode = true)
        {
            return AddTextNode(new TextNode(text, encode));
        }

        /// <summary>
        /// Adds a text node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>This instance</returns>
        public LaraBuilder AddTextNode(TextNode node)
        {
            return AddNode(node);
        }

        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>This instance</returns>
        public LaraBuilder AddNode(Node node)
        {
            var current = _stack.Peek();
            current.AppendChild(node);
            return this;
        }

        /// <summary>
        /// Adds a collection of nodes.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>This instance</returns>
        public LaraBuilder AddNodes(IEnumerable<Node> nodes)
        {
            var current = _stack.Peek();
            foreach (var node in nodes)
            {
                current.AppendChild(node);
            }
            return this;
        }

        /// <summary>
        /// Adds a collection of elements.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>This instance</returns>
        public LaraBuilder AddNodes(IEnumerable<Element> nodes)
        {
            var current = _stack.Peek();
            foreach (var node in nodes)
            {
                current.AppendChild(node);
            }
            return this;
        }

        /// <summary>
        /// Adds the elements generated by the executing the specified handler.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>This instance</returns>
        public LaraBuilder Add(Action<LaraBuilder> action)
        {
            action(this);
            return this;
        }

        /// <summary>
        /// Sets an attribute on the current element.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        /// <returns>Thsi instance</returns>
        public LaraBuilder Attribute(string attribute, string value)
        {
            var current = _stack.Peek();
            current.SetAttribute(attribute, value);
            return this;
        }

        /// <summary>
        /// Sets a flag attribute value.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        /// <returns>This instance</returns>
        public LaraBuilder FlagAttribute(string attribute, bool value)
        {
            var current = _stack.Peek();
            current.SetFlagAttribute(attribute, value);
            return this;
        }

        /// <summary>
        /// Associates an event handler with the current element.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>This instance</returns>
        public LaraBuilder On(string eventName, Func<Task> handler)
        {
            _stack.Peek().On(eventName, handler);
            return this;
        }

        /// <summary>
        /// Associates an event handler with the current element.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>This instance</returns>
        public LaraBuilder On(EventSettings settings)
        {
            _stack.Peek().On(settings);
            return this;
        }
    }
}
