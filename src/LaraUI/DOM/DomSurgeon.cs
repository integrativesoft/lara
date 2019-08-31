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

        public DomSurgeon(Element parent)
        {
            _parent = parent;
        }

        #region Public methods

        public void Append(Node child)
        {
            var previous = BeforeOperation(child);
            AppendInternal(child);
            AfterOperation(child, previous);
        }

        public void InsertChildBefore(Node reference, Node child)
        {
            var previous = BeforeOperation(child);
            InsertChild(reference, 0, child);
            AfterOperation(child, previous);
        }

        public void InsertChildAfter(Node reference, Node child)
        {
            var previous = BeforeOperation(child);
            InsertChild(reference, 1, child);
            AfterOperation(child, previous);
        }

        public void InsertChildAt(int index, Node child)
        {
            var previous = BeforeOperation(child);
            InsertChild(index, child);
            AfterOperation(child, previous);
        }

        public void Remove(Node child)
        {
            RemoveInternal(child);
            AfterOperation(child, _parent);
        }

        public void RemoveAt(int index)
        {
            var child = _parent.GetChildAt(index);
            RemoveInternal(index);
            AfterOperation(child, _parent);
        }

        public void ClearChildren()
        {
            var list = new List<Node>(_parent.Children);
            ClearChildrenInternal();
            foreach (var node in list)
            {
                AfterOperation(node, _parent);
            }
        }

        #endregion

        #region Connect and disconnect

        private Element BeforeOperation(Node child)
        {
            return child.ParentElement;
        }

        private void AfterOperation(Node node, Element previousParent)
        {
            if (node is Element child)
            {
                AfterOperation(child, previousParent);
            }
        }

        private void AfterOperation(Element child, Element previousParent)
        {
            var previousDocument = GetPreviousDocument(previousParent);
            if (previousDocument == child.Document)
            {
                if (previousDocument != null)
                {
                    NotifyMove(child);
                }
            }
            else if (child.Document == null)
            {
                NotifyDisconnected(child);
            }
            else if (previousDocument == null)
            {
                NotifyConnected(child);
            }
            else
            {
                NotifyAdopted(child);
            }            
        }

        private Document GetPreviousDocument(Element previousParent)
        {
            if (previousParent == null)
            {
                return null;
            }
            else
            {
                return previousParent.Document;
            }
        }

        private void NotifyConnected(Element child)
        {
            foreach (var item in GetBranchElements(child))
            {
                item.NotifyConnect();
            }
        }

        private void NotifyDisconnected(Element child)
        {
            foreach (var item in GetBranchElements(child))
            {
                item.NotifyDisconnect();
            }
        }

        private void NotifyMove(Element child)
        {
            foreach (var item in GetBranchElements(child))
            {
                item.NotifyMove();
            }
        }

        private void NotifyAdopted(Element child)
        {
            foreach (var item in GetBranchElements(child))
            {
                item.NotifyAdopted();
            }
        }

        private IEnumerable<Element> GetBranchElements(Element child)
        {
            yield return child;
            foreach (var item in child.GetAllDescendants())
            {
                if (item is Element element)
                {
                    yield return element;
                }
            }
        }

        #endregion

        #region Operations

        public void AppendInternal(Node child)
        {
            bool needGenerateIds = NeedsId(child);
            PreventCycles(child);
            AddImmediateId(child);
            UpdateDocumentMappings(child);
            UpdateChildParentLinks(child);
            GenerateIdsIfNeeded(needGenerateIds, child);
        }

        private void InsertChild(Node reference, int offset, Node child)
        {
            VerifyParentContainsChild(reference);
            bool needGenerateIds = NeedsId(child);
            PreventCycles(child);
            AddImmediateId(child);
            UpdateDocumentMappings(child);
            UpdateChildParentLinks(reference, offset, child);
            GenerateIdsIfNeeded(needGenerateIds, child);
        }

        private void InsertChild(int index, Node child)
        {
            bool needGenerateIds = NeedsId(child);
            PreventCycles(child);
            AddImmediateId(child);
            UpdateDocumentMappings(child);
            UpdateChildParentLinks(index, child);
            GenerateIdsIfNeeded(needGenerateIds, child);
        }

        private void VerifyParentContainsChild(Node reference)
        {
            if (!_parent.ContainsChild(reference))
            {
                throw new InvalidOperationException(ReferenceNodeNotFound);
            }
        }

        private void GenerateIdsIfNeeded(bool needGenerateIds, Node child)
        {
            if (needGenerateIds)
            {
                GenerateRequiredIds(child);
            }
        }

        private void RemoveInternal(Node child)
        {
            if (child.ParentElement != _parent)
            {
                throw new InvalidOperationException(NodeNotFoundInsideParent);
            }
            int index = _parent.GetChildNodePosition(child);
            RemoveInternalCommon(child);
            NodeRemovedDelta.Enqueue(_parent, index);
        }

        private void RemoveInternal(int index)
        {
            var child = _parent.GetChildAt(index);
            RemoveInternalCommon(child);
            NodeRemovedDelta.Enqueue(_parent, index);
        }

        private void ClearChildrenInternal()
        {
            while (_parent.ChildCount > 0)
            {
                var child = _parent.GetChildAt(_parent.ChildCount - 1);
                RemoveInternalCommon(child);
            }
            ClearChildrenDelta.Enqueue(_parent);
        }

        private void RemoveInternalCommon(Node child)
        {
            if (child is Element && _parent.Document != null)
            {
                var list = CollectNodes(child);
                RemoveFromPreviousDocument(list, _parent.Document);
            }
            _parent.OnChildRemoved(child);
            child.ParentElement = null;
        }

        #endregion

        #region Prevent cycles in DOM tree

        private void PreventCycles(Node child)
        {
            if (child is Element element && _parent.DescendsFrom(element))
            {
                throw new InvalidOperationException(CannotAddInsideItself);
            }
        }

        #endregion

        #region Update parent document and ID maps

        private void UpdateDocumentMappings(Node child)
        {
            bool newToDocument = NewToDocument(child);
            bool leavingPrevious = LeavingPrevious(child);
            if (newToDocument || leavingPrevious)
            {
                var previous = child.Document;
                var list = CollectNodes(child);
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

        private bool NewToDocument(Node child)
        {
            return _parent.Document != null
                && child.Document != _parent.Document;
        }

        private bool LeavingPrevious(Node child)
        {
            return child.Document != null
                && child.Document != _parent.Document;
        }

        private List<Node> CollectNodes(Node child)
        {
            var list = new List<Node>();
            CollectElements(list, child);
            return list;
        }

        private void CollectElements(List<Node> list, Node node)
        {
            list.Add(node);
            if (node is Element element)
            {
                foreach (var child in element.GetAllDescendants())
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

        private void UpdateChildParentLinks(Node child)
        {
            child.ParentElement?.OnChildRemoved(child);
            _parent.OnChildAppend(child);
            child.ParentElement = _parent;
            NodeAddedDelta.Enqueue(child);
        }

        private void UpdateChildParentLinks(Node reference, int offset, Node child)
        {
            int index = _parent.GetChildNodePosition(reference) + offset;
            UpdateChildParentLinks(index, child);
        }

        private void UpdateChildParentLinks(int index, Node child)
        {
            child.ParentElement?.OnChildRemoved(child);
            _parent.OnChildInsert(index, child);
            child.ParentElement = _parent;
            NodeInsertedDelta.Enqueue(child, index);
        }

        #endregion

        #region Generate IDs for elements with events

        private bool NeedsId(Node child)
        {
            return child is Element
                && child.Document == null
                && _parent.Document != null;
        }

        private void AddImmediateId(Node child)
        {
            if (child is Element element
                && _parent.TagName == "body")
            {
                element.EnsureElementId();
            }
        }

        private static void GenerateRequiredIds(Node node)
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
