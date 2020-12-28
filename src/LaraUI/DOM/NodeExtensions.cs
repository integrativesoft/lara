/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using System;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Extensions for Node class
    /// </summary>
    public static class NodeExtensions
    {
        #region Generic wrapping of methods

        /// <summary>
        /// Executes an action on an object and returns the object itself
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <param name="actions"></param>
        /// <returns>object invoked</returns>
        public static TNode Wrap<TNode>(this TNode node, params Action<TNode>[] actions)
        {
            foreach (var action in actions)
            {
                action(node);
            }
            return node;
        }

        /// <summary>
        /// Returns the object itself
        /// </summary>
        /// <typeparam name="TNode">type of node</typeparam>
        /// <param name="node">node</param>
        /// <param name="result">node itself</param>
        /// <returns>node itself</returns>
        public static TNode Extract<TNode>(this TNode node, out TNode result)
        {
            result = node;
            return node;
        }

        #endregion

        #region add children

        /// <summary>
        /// Appends multiple children and returns the element passed as parameter
        /// </summary>
        /// <typeparam name="T">Type of parent</typeparam>
        /// <param name="element">parent</param>
        /// <param name="elements">child nodes</param>
        /// <returns></returns>
        public static T Child<T>(this T element, params Node[] elements)
            where T : Element
        {
            element.AppendChild(elements);
            return element;
        }

        #endregion

        #region add events

        /// <summary>
        /// Registers an event and associates code to execute
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="element">Element</param>
        /// <param name="eventName">Event name (e.g. "click")</param>
        /// <param name="handler">Code to execute</param>
        /// <returns>Element instance</returns>
        public static T Event<T>(this T element, string eventName, Action handler)
            where T : Element
        {
            element.On(eventName, handler);
            return element;
        }

        /// <summary>
        /// Registers an event and associates code to execute
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="element">Element</param>
        /// <param name="eventName">Event name (e.g. "click")</param>
        /// <param name="handler">Code to execute</param>
        /// <returns>Element instance</returns>
        public static T Event<T>(this T element, string eventName, Func<Task> handler)
            where T : Element
        {
            element.On(eventName, handler);
            return element;
        }

        /// <summary>
        /// Registers an event and associates code to execute
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="element">Element</param>
        /// <param name="options">Event options</param>
        /// <returns>Element instance</returns>
        public static T Event<T>(this T element, EventSettings options)
            where T : Element
        {
            element.On(options);
            return element;
        }

        #endregion
    }
}
