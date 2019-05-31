/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using System;
using System.Collections.Generic;

namespace Integrative.Lara.DOM
{
    sealed class DomSurgeon
    {
        const string CannotAddInsideItself = "Cannot add an element inside itself.";
        const string ReferenceNodeNotFound = "Reference before/after node not found";
        const string NodeNotFoundInsideParent = "Invalid child/parent nodes specified";

        readonly Element _parent;
        readonly Node _child;

        public DomSurgeon(Element parent, Node child)
        {
            _parent = parent;
            _child = child;
        }

        #region Public methods

        public void Append()
        {
            AppendInternal();
        }

        public void InsertChildBefore(Node reference)
        {
            InsertChild(reference, 0);
        }

        public void InsertChildAfter(Node reference)
        {
            InsertChild(reference, 1);
        }

        public void Remove()
        {
            RemoveInternal();
        }

        #region Operations

        public void AppendInternal()
        {
            bool needGenerateIds = NeedsGenerateIdsEvents();
            PreventCycles();
            UpdateDocumentMappings();
            UpdateChildParentLinks();
            if (needGenerateIds)
            {
                GenerateRequiredIds();
            }
        }

        private void InsertChild(Node reference, int offset)
        {
            if (!_parent.ContainsChild(reference))
            {
                throw new InvalidOperationException(ReferenceNodeNotFound);
            }
            bool needGenerateIds = NeedsGenerateIdsEvents();
            PreventCycles();
            UpdateDocumentMappings();
            UpdateChildParentLinks(reference, offset);
            if (needGenerateIds)
            {
                GenerateRequiredIds();
            }
        }

        private void RemoveInternal()
        {
            if (_child.ParentElement != _parent)
            {
                throw new InvalidOperationException(NodeNotFoundInsideParent);
            }
            int index = _parent.GetChildNodePosition(_child);
            if (_child is Element && _parent.Document != null)
            {
                var list = CollectNodes();
                RemoveFromPreviousDocument(list, _parent.Document);
            }
            _parent.OnChildRemoved(_child);
            _child.ParentElement = null;
            NodeRemovedDelta.Enqueue(_parent, index);
        }

        #endregion

        #endregion

        #region Prevent cycles in DOM tree

        private void PreventCycles()
        {
            if (_child is Element element && _parent.DescendsFrom(element))
            {
                throw new InvalidOperationException(CannotAddInsideItself);
            }
        }

        #endregion

        #region Update parent document and ID maps

        private void UpdateDocumentMappings()
        {
            bool newToDocument = NewToDocument();
            bool leavingPrevious = LeavingPrevious();
            if (newToDocument || leavingPrevious)
            {
                var previous = _child.Document;
                var list = CollectNodes();
                if (leavingPrevious)
                {
                    RemoveFromPreviousDocument(list, previous);
                }
                if (newToDocument)
                {
                    PreventDuplicateIds(list);
                    AddToDocument(list);
                }                
            }
        }

        private bool NewToDocument()
        {
            return _parent.Document != null
                && _child.Document != _parent.Document;
        }

        private bool LeavingPrevious()
        {
            return _child.Document != null
                && _child.Document != _parent.Document;
        }

        private List<Node> CollectNodes()
        {
            var list = new List<Node>();
            CollectElements(list, _child);
            return list;
        }

        private void CollectElements(List<Node> list, Node node)
        {
            list.Add(node);
            if (node is Element element)
            {
                foreach (var child in element.Children)
                {
                    CollectElements(list, child);
                }
            }
        }

        private void PreventDuplicateIds(List<Node> list)
        {
            var document = _parent.Document;
            var hash = new HashSet<string>();
            foreach (var node in list)
            {
                if (node is Element element
                    && !string.IsNullOrEmpty(element.Id))
                {
                    string id = element.Id;
                    if (hash.Contains(id) || document.TryGetElementById(id, out _))
                    {
                        throw DuplicateElementId.Create(id);
                    }
                    hash.Add(id);
                }
            }
        }

        private void AddToDocument(List<Node> list)
        {
            var document = _parent.Document;
            foreach (var node in list)
            {
                node.Document = document;
                if (node is Element element)
                {
                    document.OnElementAdded(element);
                }
            }
        }

        private void RemoveFromPreviousDocument(List<Node> list, Document document)
        {
            foreach (var node in list)
            {
                if (node is Element element)
                {
                    document.OnElementRemoved(element);
                }
                node.Document = null;
            }
        }

        #endregion

        #region Update Children collections and ParentElement reference

        private void UpdateChildParentLinks()
        {
            _child.ParentElement?.OnChildRemoved(_child);
            _parent.OnChildAppend(_child);
            _child.ParentElement = _parent;
            NodeAddedDelta.Enqueue(_child);
        }

        private void UpdateChildParentLinks(Node reference, int offset)
        {
            _child.ParentElement?.OnChildRemoved(_child);
            int index = _parent.GetChildNodePosition(reference) + offset;
            _parent.OnChildInsert(index, _child);
            _child.ParentElement = _parent;
            NodeInsertedDelta.Enqueue(_child, index);
        }

        #endregion

        #region Generate IDs for elements with events

        private bool NeedsGenerateIdsEvents()
        {
            return _child is Element
                && _child.Document == null
                && _parent.Document != null;
        }

        private void GenerateRequiredIds()
        {
            GenerateRequiredIds(_child);
        }

        private void GenerateRequiredIds(Node node)
        {
            if (node is Element element)
            {
                if (element.NeedsId)
                {
                    element.EnsureElementId();
                }
                foreach (var child in element.Children)
                {
                    GenerateRequiredIds(child);
                }
            }
        }

        #endregion
    }
}
