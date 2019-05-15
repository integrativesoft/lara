/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Delta;
using Integrative.Clara.Main;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Integrative.Clara.DOM
{
    public class Element : Node
    {
        private readonly Attributes _attributes;
        private readonly List<Node> _children;
        private readonly Dictionary<string, Func<IPageContext, Task>> _events;

        private string _id;
        public string TagName { get; }        

        public Element(string tagName) : this(null, tagName)
        {
        }

        internal Element(Document document, string tagName)
            : base(document)
        {
            _attributes = new Attributes(this);
            _children = new List<Node>();
            _events = new Dictionary<string, Func<IPageContext, Task>>();
            TagName = tagName.ToLowerInvariant();
        }

        public override NodeType NodeType => NodeType.Element;

        #region Attributes

        public string Id
        {
            get => _id;
            set
            {
                Document?.NotifyChangeId(this, _id, value);
                _id = value;
                if (value is null)
                {
                    _attributes.RemoveAttributeLower("id");
                }
                else
                {
                    _attributes.SetAttributeLower("id", value);
                }
            }
        }

        public string EnsureElementId()
        {
            if (string.IsNullOrEmpty(_id))
            {
                var document = Document ?? throw new InvalidOperationException("Cannot use EnsureElementId on orphan elements");
                Id = document.GenerateElementId();
            }
            return Id;
        }

        public void SetAttribute(string name, string value)
        {
            name = name.ToLower();
            if (name == "id")
            {
                Id = value;
            }
            else
            {
                _attributes.SetAttributeLower(name, value);
            }
        }

        public bool HasAttribute(string name) => _attributes.HasAttribute(name);

        public string GetAttribute(string name) => _attributes.GetAttribute(name);

        public void RemoveAttribute(string name)
        {
            name = name.ToLower();
            if (name == "id")
            {
                Id = null;
            }
            else
            {
                _attributes.RemoveAttributeLower(name);
            }
        }

        public IEnumerable<KeyValuePair<string, string>> Attributes => _attributes;

        #endregion

        #region Global attributes

        public string AccessKey
        {
            get => _attributes.GetAttributeLower("accesskey");
            set { _attributes.SetAttributeLower("accesskey", value); }
        }

        public string Class
        {
            get => _attributes.GetAttributeLower("class");
            set { _attributes.SetAttributeLower("class", value); }
        }

        public string ContentEditable
        {
            get => _attributes.GetAttributeLower("contenteditable");
            set { _attributes.SetAttributeLower("contenteditable", value); }
        }

        public string Dir
        {
            get => _attributes.GetAttributeLower("dir");
            set { _attributes.SetAttributeLower("dir", value); }
        }

        public string Draggable
        {
            get => _attributes.GetAttributeLower("draggable");
            set { _attributes.SetAttributeLower("draggable", value); }
        }

        public string DropZone
        {
            get => _attributes.GetAttributeLower("dropzone");
            set { _attributes.SetAttributeLower("dropzone", value); }
        }

        public bool Hidden
        {
            get => _attributes.HasAttributeLower("hiddden");
            set { _attributes.SetFlagAttributeLower("hidden", value); }
        }

        public string Lang
        {
            get => _attributes.GetAttributeLower("lang");
            set { _attributes.SetAttributeLower("lang", value); }
        }

        public string Spellcheck
        {
            get => _attributes.GetAttributeLower("spellcheck");
            set { _attributes.SetAttributeLower("spellcheck", value); }
        }

        public string Style
        {
            get => _attributes.GetAttributeLower("style");
            set { _attributes.SetAttributeLower("style", value); }
        }

        public string TabIndex
        {
            get => _attributes.GetAttributeLower("tabindex");
            set { _attributes.SetAttributeLower("tabindex", value); }
        }

        public string Title
        {
            get => _attributes.GetAttributeLower("title");
            set { _attributes.SetAttributeLower("title", value); }
        }

        public string Translate
        {
            get => _attributes.GetAttributeLower("translate");
            set { _attributes.SetAttributeLower("translate", value); }
        }

        #endregion

        #region DOM tree queries

        public IEnumerable<Node> Children => _children;

        public int ChildCount => _children.Count;

        public Node GetChildAt(int index) => _children[index];

        public int GetChildPosition(Node node)
        {
            int index = 0;
            foreach (var child in _children)
            {
                if (child == node)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        public bool DescendsFrom(Element element)
        {
            if (element == null || ParentElement == null)
            {
                return false;
            }
            else if (ParentElement == element)
            {
                return true;
            }
            else
            {
                return ParentElement.DescendsFrom(element);
            }
        }

        #endregion

        #region DOM operations

        public void AppendChild(Node node)
        {
            PreventCycles(node);
            node.ParentElement?.RemoveChild(node);
            _children.Add(node);
            node.ParentElement = this;
            node.Document = Document;
            EnsureIdChildrenWithEvents();
            NodeAddedDelta.Enqueue(node);
        }

        public void InsertChildBefore(Node before, Node node)
        {
            InsertChildRelative(before, 0, node);
        }

        public void InsertChildAfter(Node after, Node node)
        {
            InsertChildRelative(after, 1, node);
        }

        private void InsertChildRelative(Node reference, int offset, Node node)
        {
            PreventCycles(node);
            node.ParentElement?.RemoveChild(node);
            int index = GetChildPosition(reference);
            VerifyNodeFound(index);
            ExecuteInsertChild(index + offset, node);
        }

        public void InsertChildAt(int index, Node node)
        {
            PreventCycles(node);
            node.ParentElement?.RemoveChild(node);
            ExecuteInsertChild(index, node);
        }

        private void ExecuteInsertChild(int index, Node node)
        {
            _children.Insert(index, node);
            node.ParentElement = this;
            node.Document = Document;
            EnsureIdChildrenWithEvents();
            NodeInsertedDelta.Enqueue(node, index);
        }

        private void PreventCycles(Node child)
        {
            if (child is Element element && DescendsFrom(element))
            {
                throw new InvalidOperationException("Cannot add an element inside itself.");
            }
        }

        internal static void VerifyNodeFound(int index)
        {
            if (index == -1)
            {
                throw new InvalidOperationException("Node not found within specified parent.");
            }
        }

        public void RemoveChild(Node child)
        {
            int index = GetChildPosition(child);
            if (index > -1)
            {
                _children.RemoveAt(index);
                child.ParentElement = null;
                child.Document = null;
                NodeRemovedDelta.Enqueue(this, index);
            }
            else
            {
                throw new InvalidOperationException("The specified node isn't a child of the specified parent.");
            }
        }

        public void Remove()
        {
            ParentElement?.RemoveChild(this);
        }

        protected override void SetDocument(Document newDocument)
        {
            if (newDocument != Document)
            {
                Document?.OnElementRemoved(this);
                newDocument?.OnElementAdded(this);
                foreach (var child in _children)
                {
                    child.Document = newDocument;
                }
            }
            base.SetDocument(newDocument);
        }

        internal void NotifyValue(string value)
        {
            _attributes.NotifyValue(value);
        }

        #endregion

        #region Generate Delta content

        internal override ContentNode GetContentNode()
        {
            return new ContentElementNode
            {
                TagName = TagName,
                Attributes = CopyAttributes(),
                Children = CopyChildren()
            };
        }

        private List<ContentAttribute> CopyAttributes()
        {
            var list = new List<ContentAttribute>();
            foreach (var pair in _attributes)
            {
                list.Add(new ContentAttribute
                {
                    Attribute = pair.Key,
                    Value = pair.Value
                });
            }
            return list;
        }

        private List<ContentNode> CopyChildren()
        {
            var list = new List<ContentNode>();
            foreach (var child in _children)
            {
                list.Add(child.GetContentNode());
            }
            return list;
        }

        #endregion

        #region Subscribe to events

        internal async Task NotifyEvent(string eventName, IPageContext context)
        {
            if (_events.TryGetValue(eventName, out var handler))
            {
                await handler(context);
            }
        }

        public void On(string eventName, Func<IPageContext, Task> handler)
        {
            eventName = HttpUtility.HtmlEncode(eventName.ToLowerInvariant());
            _events.Remove(eventName);
            if (handler == null)
            {
                string attribute = GetEventAttribute(eventName);
                RemoveAttribute(attribute);
            }
            else
            {
                _events.Add(eventName, handler);
                WriteEvent(eventName);
            }
        }

        private void WriteEvent(string eventName)
        {
            string attribute = GetEventAttribute(eventName);
            string value = $"ClaraUI.plug(this, '{eventName}');";
            SetAttribute(attribute, value);
        }

        private static string GetEventAttribute(string eventName)
        {
            return "on" + eventName;
        }

        private void EnsureIdChildrenWithEvents()
        {
            if (_events.Count > 0 && string.IsNullOrEmpty(_id))
            {
                EnsureElementId();
            }
            foreach (var child in _children)
            {
                if (child is Element element)
                {
                    element.EnsureElementId();
                }
            }
        }

        #endregion
    }
}
