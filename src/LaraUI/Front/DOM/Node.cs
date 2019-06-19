/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;

namespace Integrative.Lara
{
    public enum NodeType
    {
        Document,
        Element,
        Text
    }

    /// <summary>
    /// An abstract class that represents a node in an HTML5 document
    /// </summary>
    public abstract class Node
    {
        protected Node()
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
        /// The node's document. This proprety returns null when the node is not attached to a document.
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

        internal bool QueueOpen =>
            Document != null && Document.QueueOpen;
    }
}
