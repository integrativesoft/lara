/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Delta;

namespace Integrative.Clara.DOM
{
    public enum NodeType
    {
        Document,
        Element,
        Text
    }

    public abstract class Node
    {
        protected Node()
        {
        }

        public Element ParentElement { get; internal set; }
        public Document Document { get; internal set; }

        public abstract NodeType NodeType { get; }

        internal abstract ContentNode GetContentNode();

        internal bool QueueOpen =>
            Document != null && Document.QueueOpen;

        internal virtual void UpdateBranchDocument(Document document)
        {
        }
    }
}
