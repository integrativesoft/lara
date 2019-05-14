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
        Document _document;

        public Document Document
        {
            get => _document;
            set
            {
                SetDocument(value);
            }
        }

        protected virtual void SetDocument(Document newDocument)
        {
            _document = newDocument;
        }

        public Element ParentElement { get; internal set; }

        public abstract NodeType NodeType { get; }

        protected Node(Document document)
        {
            _document = document;
        }

        internal abstract ContentNode GetContentNode();

        internal virtual bool QueueOpen =>
            _document != null && _document.QueueOpen;
    }
}
