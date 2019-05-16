/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using Integrative.Lara.Main;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Integrative.Lara.DOM
{
    public class Element : Node
    {
        private const string FocusAllowedOnlyInEvents = "The Focus() method is not allowed during the page's initial GET, and only within events.";
        private readonly Attributes _attributes;
        private readonly List<Node> _children;
        private readonly Dictionary<string, Func<IPageContext, Task>> _events;

        private string _id;
        public string TagName { get; }

        public Element(string tagName, string id) : this(tagName)
        {
            Id = id;
        }

        public Element(string tagName)
            : base()
        {
            _attributes = new Attributes(this);
            _children = new List<Node>();
            _events = new Dictionary<string, Func<IPageContext, Task>>();
            TagName = tagName.ToLowerInvariant();
        }

        public override NodeType NodeType => NodeType.Element;

        internal bool NeedsId => string.IsNullOrEmpty(_id)
            && (_events.Count > 0 || HtmlReference.RequiresId(TagName));

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_id))
            {
                return TagName;
            }
            else
            {
                return TagName + $" id='{_id}'";
            }
        }

        #region Attributes

        public string Id
        {
            get => _id;
            set
            {
                if (value == _id)
                {
                    return;
                }
                Document?.NotifyChangeId(this, _id, value);
                if (value is null)
                {
                    _attributes.RemoveAttributeLower("id");
                }
                else
                {
                    _attributes.SetAttributeLower("id", value);
                }
                _id = value;
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
            get => _attributes.HasAttributeLower("hidden");
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

        public bool ContainsChild(Node node)
        {
            return _children.Contains(node);
        }

        #endregion

        #region DOM operations

        internal void OnChildRemoved(Node child)
        {
            _children.Remove(child);
        }

        internal void OnChildAppend(Node child)
        {
            _children.Add(child);
        }

        internal void OnChildInsert(int index, Node child)
        {
            _children.Insert(index, child);
        }

        public void AppendChild(Node node)
        {
            var append = new DomSurgeon(this, node);
            append.Append();
        }

        public void InsertChildBefore(Node before, Node node)
        {
            var append = new DomSurgeon(this, node);
            append.InsertChildBefore(before);
        }

        public void InsertChildAfter(Node after, Node node)
        {
            var append = new DomSurgeon(this, node);
            append.InsertChildAfter(after);
        }

        public void RemoveChild(Node child)
        {
            var remover = new DomSurgeon(this, child);
            remover.Remove();
        }

        public void Remove()
        {
            if (ParentElement == null)
            {
                throw new InvalidOperationException("Cannot remove from parent, the node has no parent element already");
            }
            ParentElement.RemoveChild(this);
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
            string value = $"LaraUI.plug(this, '{eventName}');";
            SetAttribute(attribute, value);
        }

        private static string GetEventAttribute(string eventName)
        {
            return "on" + eventName;
        }

        #endregion

        #region Other methods

        public void Focus()
        {
            if (!QueueOpen)
            {
                throw new InvalidOperationException(FocusAllowedOnlyInEvents);
            }
            FocusDelta.Enqueue(this);
        }

        #endregion
    }
}
