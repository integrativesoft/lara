/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Components;
using Integrative.Lara.Delta;

namespace Integrative.Lara
{
    /// <summary>
    /// Defines the types of document nodes
    /// </summary>
    public enum NodeType
    {
        /// <summary>
        /// Element node
        /// </summary>
        Element,

        /// <summary>
        /// Text node
        /// </summary>
        Text
    }

    /// <summary>
    /// An abstract class that represents a node in an HTML5 document
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// Creates a node
        /// </summary>
        internal protected Node()
        {
        }

        /// <summary>
        /// The node's parent element.
        /// </summary>
        /// <value>
        /// The parent element.
        /// </value>
        public Element ParentElement { get; internal set; }

        /// <summary>
        /// The node's document. This property returns null when the node is not attached to a document.
        /// </summary>
        /// <value>
        /// The document.
        /// </value>
        public Document Document { get; internal set; }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>
        /// The type of the node.
        /// </value>
        public abstract NodeType NodeType { get; }

        internal abstract ContentNode GetContentNode();

        /// <summary>
        /// True when the node is currently rendered in a parent document.
        /// </summary>
        public bool IsSlotted { get; internal set; }

        internal void UpdateSlotted()
        {
            SlottedCalculator.UpdateSlotted(this);
        }

        internal virtual bool IsPrintable => true;
    }
}
