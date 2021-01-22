/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Integrative.Lara
{
    internal sealed class DomSurgeon
    {
        private readonly Element _parent;

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

        private static Element? BeforeOperation(Node child)
        {
            return child.ParentElement;
        }

        private static void AfterOperation(Node node, Node? previousParent)
        {
            if (node is Element child)
            {
                AfterOperationInternal(child, previousParent);
            }
        }

        private static void AfterOperationInternal(Element child, Node? previousParent)
        {
            var previousDocument = GetPreviousDocument(previousParent);
            if (previousDocument == child.Document)
            {
                if (previousDocument != null)
                {
                    child.NotifyMove();
                }
            }
            else if (child.Document == null)
            {
                child.NotifyDisconnect();
            }
            else if (previousDocument == null)
            {
                child.NotifyConnect();
            }
            else
            {
                child.NotifyAdopted();
            }            
        }

        private static Document? GetPreviousDocument(Node? previousParent)
        {
            return previousParent?.Document;
        }

        #endregion

        #region Operations

        private void AppendInternal(Node child)
        {
            PreventCycles(child);
            UpdateDocumentMappings(child);
            UpdateChildParentLinks(child);
        }

        private void InsertChild(Node reference, int offset, Node child)
        {
            VerifyParentContainsChild(reference);
            PreventCycles(child);
            UpdateDocumentMappings(child);
            UpdateChildParentLinks(reference, offset, child);
        }

        private void InsertChild(int index, Node child)
        {
            PreventCycles(child);
            UpdateDocumentMappings(child);
            UpdateChildParentLinks(index, child);
        }

        private void VerifyParentContainsChild(Node reference)
        {
            if (!_parent.ContainsChild(reference))
            {
                throw new InvalidOperationException(Resources.ReferenceNodeNotFound);
            }
        }

        private void RemoveInternal(Node child)
        {
            if (child.ParentElement != _parent)
            {
                throw new InvalidOperationException(Resources.NodeNotFoundInsideParent);
            }
            if (child is Element childElement)
            {
                RemoveElementDelta.Enqueue(childElement);
                RemoveInternalCommon(child);
            }
            else
            {
                var index = _parent.GetChildNodePosition(child);
                RemoveInternalCommon(child);
                NodeRemovedDelta.Enqueue(_parent, index);
            }
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
            child.UpdateSlotted();
        }

        #endregion

        #region Prevent cycles in DOM tree

        private void PreventCycles(Node child)
        {
            if (child is Element element && _parent.DescendsFrom(element))
            {
                throw new InvalidOperationException(Resources.CannotAddInsideItself);
            }
        }

        #endregion

        #region Update parent document and ID maps

        private void UpdateDocumentMappings(Node child)
        {
            var newToDocument = NewToDocument(child, out var newDocument);
            var leavingPrevious = LeavingPrevious(child, out var previous);
            if (!newToDocument && !leavingPrevious) return;
            var list = CollectNodes(child);
            if (leavingPrevious)
            {
                RemoveFromPreviousDocument(list, previous!);
            }

            if (!newToDocument) return;
            PreventDuplicateIds(list);
            AddToDocument(list, newDocument!);
        }

        private bool NewToDocument(Node child, [NotNullWhen(true)] out Document? newDocument)
        {
            newDocument = _parent.Document;
            return newDocument != null
                   && child.Document != newDocument;
        }

        private bool LeavingPrevious(Node child, [NotNullWhen(true)] out Document? previousDocument)
        {
            previousDocument = child.Document;
            return previousDocument != null
                   && previousDocument != _parent.Document;
        }

        private static List<Node> CollectNodes(Node child)
        {
            var list = new List<Node>();
            CollectElements(list, child);
            return list;
        }

        private static void CollectElements(ICollection<Node> list, Node node)
        {
            list.Add(node);
            if (node is not Element element) return;
            foreach (var child in element.GetAllDescendants())
            {
                CollectElements(list, child);
            }
        }

        private void PreventDuplicateIds(IEnumerable<Node> list)
        {
            var document = _parent.Document;
            var hash = new HashSet<string>();
            foreach (var node in list)
            {
                if (node is not Element element || string.IsNullOrEmpty(element.Id)) continue;
                var id = element.Id;
                if (hash.Contains(id) || DuplicateIdInDocument(document, id))
                {
                    throw DuplicateElementIdException.Create(id);
                }
                hash.Add(id);
            }
        }

        private static bool DuplicateIdInDocument(Document? document, string id)
        {
            return document != null
                && document.TryGetElementById(id, out _);
        }

        private static void AddToDocument(IEnumerable<Node> list, Document document)
        {
            foreach (var node in list)
            {
                node.Document = document;
                if (node is Element element)
                {
                    document.OnElementAdded(element);
                }
            }
        }

        private static void RemoveFromPreviousDocument(IEnumerable<Node> list, Document document)
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
            child.UpdateSlotted();
            NodeAddedDelta.Enqueue(child);
        }

        private void UpdateChildParentLinks(Node reference, int offset, Node child)
        {
            var index = _parent.GetChildNodePosition(reference) + offset;
            UpdateChildParentLinks(index, child);
        }

        private void UpdateChildParentLinks(int index, Node child)
        {
            child.ParentElement?.OnChildRemoved(child);
            _parent.OnChildInsert(index, child);
            child.ParentElement = _parent;
            child.UpdateSlotted();
            NodeInsertedDelta.Enqueue(child, index);
        }

        #endregion
    }
}
