/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// An Element node inside an HTML5 document
    /// </summary>
    /// <seealso cref="Node" />
    public abstract class Element : Node
    {
        private readonly Attributes _attributes;
        private readonly List<Node> _children;

        internal Dictionary<string, EventSettings> Events { get; }

        private string _id;

        /// <summary>
        /// Element's tag name
        /// </summary>
        /// <value>
        /// The element's tag name
        /// </value>
        public string TagName { get; }

        /// <summary>
        /// Creates an element
        /// </summary>
        /// <param name="tagName">Element's tag name.</param>
        /// <returns>Element created</returns>
        public static Element Create(string tagName) => ElementFactory.CreateElement(tagName);

        /// <summary>
        /// Creates an element
        /// </summary>
        /// <param name="tagName">Element's tag name.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>Element created</returns>
        public static Element Create(string tagName, string id) => ElementFactory.CreateElement(tagName, id);

        /// <summary>
        /// Creates a namespace-specific HTML5 element (e.g. SVG elements)
        /// </summary>
        /// <param name="ns">The namespace of the element</param>
        /// <param name="tagName">Element's tag name.</param>
        /// <returns>Element created</returns>
        // ReSharper disable once InconsistentNaming
        public static Element CreateNS(string ns, string tagName) => ElementFactory.CreateElementNs(ns, tagName);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tagName">element tag</param>
        protected Element(string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
            {
                tagName = GetDefaultTagName(GetType());
            }
            TagName = tagName.ToLowerInvariant();
            _id = GlobalSerializer.GenerateElementId();
            _attributes = new Attributes(this);
            _children = new List<Node>();
            Events = new Dictionary<string, EventSettings>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected Element() : this("")
        {
        }

        /// <summary>
        /// Returns a default/suggested tag name for a type
        /// </summary>
        /// <param name="type">object type</param>
        /// <returns>default/suggested tag name</returns>
        public static string GetDefaultTagName(Type type)
        {
            var name = type.FullName ?? throw new ArgumentException("Invalid type name");
            return name.Replace('.', '-').ToLowerInvariant();   
        }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>
        /// The type of the node.
        /// </value>
        public sealed override NodeType NodeType => NodeType.Element;

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var suffix = Class;
            if (string.IsNullOrEmpty(suffix))
            {
                suffix = string.Empty;
            }
            else
            {
                suffix = " " + suffix;
            }
            return string.IsNullOrEmpty(_id) ? TagName : $"{TagName} #{_id}{suffix}";
        }

        /// <summary>
        /// Returns the ID by assigning one if needed
        /// </summary>
        /// <returns></returns>
        [Obsolete("Not needed anymore")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string EnsureElementId() => Id;

        #region Attributes

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id
        {
            get => _id;
            set
            {
                if (value == _id)
                {
                    return;
                }
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new InvalidOperationException("Element IDs cannot be empty");
                }
                Document?.NotifyChangeId(this, _id, value);
                _attributes.SetAttributeLower("id", value);
                _id = value;
            }
        }

        /// <summary>
        /// Sets an attribute and its value.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="attributeValue">The value of the attribute.</param>
        public void SetAttribute(string attributeName, string? attributeValue)
        {
            attributeName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
            SetAttributeLower(attributeName.ToLowerInvariant(), attributeValue);
        }

        internal void SetAttributeLower(string nameLower, string? value)
        {
            if (nameLower == "id")
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Element IDs cannot be empty");
                }
                Id = value;
            }
            else
            {
                _attributes.SetAttributeLower(nameLower, value);
            }
        }

        internal void ToggleAttributeLower(string nameLower, bool value)
        {
            if (value)
            {
                SetAttributeLower(nameLower, "");
            }
            else
            {
                RemoveAttribute(nameLower);
            }
        }

        /// <summary>
        /// Determines whether the element has the given attribute
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>
        ///   <c>true</c> if the element has the specified attribute; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAttribute(string attributeName)
        {
            attributeName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
            return _attributes.HasAttribute(attributeName);
        }

        internal bool HasAttributeLower(string nameLower)
            => _attributes.HasAttributeLower(nameLower);

        /// <summary>
        /// Gets the value of an attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>Value of the attribute</returns>
        public string? GetAttribute(string attributeName)
        {
            attributeName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
            return _attributes.GetAttribute(attributeName);
        }

        internal string? GetAttributeLower(string nameLower)
            => _attributes.GetAttributeLower(nameLower);

        /// <summary>
        /// Adds or removes a flag attribute
        /// </summary>
        /// <param name="attributeName">Attribute's name</param>
        /// <param name="value">true to add, false to remove</param>
        public void SetFlagAttribute(string attributeName, bool value)
        {
            attributeName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
            _attributes.SetFlagAttributeLower(attributeName.ToLowerInvariant(), value);
        }

        internal void SetFlagAttributeLower(string nameLower, bool value)
            => _attributes.SetFlagAttributeLower(nameLower, value);

        /// <summary>
        /// Removes an attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        public void RemoveAttribute(string attributeName)
        {
            attributeName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
            attributeName = attributeName.ToLowerInvariant();
            if (attributeName == "id")
            {
                throw new InvalidOperationException("Cannot remove element ID attribute");
            }

            _attributes.RemoveAttributeLower(attributeName);
        }

        internal int? GetIntAttribute(string nameLower)
        {
            if (int.TryParse(GetAttributeLower(nameLower), out var value))
            {
                return value;
            }

            return null;
        }

        internal void SetIntAttribute(string nameLower, int? value)
        {
            if (value == null)
            {
                _attributes.RemoveAttributeLower(nameLower);
            }
            else
            {
                var text = ((int)value).ToString(CultureInfo.InvariantCulture);
                SetAttributeLower(nameLower, text);
            }
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>
        /// The attributes.
        /// </value>
        public IEnumerable<KeyValuePair<string, string>> Attributes => _attributes;

        /// <summary>
        /// Determines whether the 'class' attribute contains the specified class.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns>
        ///   <c>true</c> if the specified class is found; otherwise, <c>false</c>.
        /// </returns>
        public bool HasClass(string className) => ClassEditor.HasClass(Class, className);

        /// <summary>
        /// Adds the given class name to the 'class' attribute.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        public void AddClass(string className) => Class = ClassEditor.AddClass(Class, className);

        /// <summary>
        /// Removes the given class name from the 'class' attribute.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        public void RemoveClass(string className) => Class = ClassEditor.RemoveClass(Class, className);

        /// <summary>
        /// Adds or removes the given class name from the 'class' attribute.
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="value">true to add the class, false to remove.</param>
        public void ToggleClass(string className, bool value)
            => Class = ClassEditor.ToggleClass(Class, className, value);

        /// <summary>
        /// Toggles (adds or removes) the class passed in parameters
        /// </summary>
        /// <param name="className">Class name to toggle</param>
        public void ToggleClass(string className) => Class = ClassEditor.ToggleClass(Class, className);

        internal void NotifyValue(string value) => _attributes.NotifyValue(value);

        internal void NotifyChecked(bool value) => _attributes.NotifyChecked(value);

        internal void NotifySelected(bool value) => _attributes.NotifySelected(value);

        #endregion

        #region Global attributes

        /// <summary>
        /// The 'accesskey' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The access key.
        /// </value>
        public virtual string? AccessKey
        {
            get => GetAttributeLower("accesskey");
            set => SetAttributeLower("accesskey", value);
        }

        /// <summary>
        /// The 'autocapitalize' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The automatic capitalize.
        /// </value>
        // ReSharper disable once UnusedMember.Global
        public virtual string? AutoCapitalize
        {
            get => GetAttributeLower("autocapitalize");
            set => SetAttributeLower("autocapitalize", value);
        }

        /// <summary>
        /// The 'class' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The class.
        /// </value>
        public virtual string? Class
        {
            get => GetAttributeLower("class");
            set => SetAttributeLower("class", value);
        }

        /// <summary>
        /// The 'contenteditable' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The content editable.
        /// </value>
        public virtual string? ContentEditable
        {
            get => GetAttributeLower("contenteditable");
            set => SetAttributeLower("contenteditable", value);
        }

        /// <summary>
        /// The 'contextmenu' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The context menu.
        /// </value>
        // ReSharper disable once UnusedMember.Global
        public virtual string? ContextMenu
        {
            get => GetAttributeLower("contextmenu");
            set => SetAttributeLower("contextmenu", value);
        }

        /// <summary>
        /// The 'dir' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The dir.
        /// </value>
        public virtual string? Dir
        {
            get => GetAttributeLower("dir");
            set => SetAttributeLower("dir", value);
        }

        /// <summary>
        /// The 'draggable' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The draggable.
        /// </value>
        public virtual string? Draggable
        {
            get => GetAttributeLower("draggable");
            set => SetAttributeLower("draggable", value);
        }

        /// <summary>
        /// The 'dropzone' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The drop zone.
        /// </value>
        public virtual string? DropZone
        {
            get => GetAttributeLower("dropzone");
            set => SetAttributeLower("dropzone", value);
        }

        /// <summary>
        /// The 'hidden' HTML5 attribute.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hidden; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Hidden
        {
            get => HasAttributeLower("hidden");
            set => SetFlagAttributeLower("hidden", value);
        }

        /// <summary>
        /// The 'inputmode' HTML5 attribute.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [input mode]; otherwise, <c>false</c>.
        /// </value>
        // ReSharper disable once UnusedMember.Global
        public virtual bool InputMode
        {
            get => HasAttributeLower("inputmode");
            set => SetFlagAttributeLower("inputmode", value);
        }

        /// <summary>
        /// The 'lang' HTML5 attribute
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public virtual string? Lang
        {
            get => GetAttributeLower("lang");
            set => SetAttributeLower("lang", value);
        }

        /// <summary>
        /// The 'spellcheck' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The spellcheck.
        /// </value>
        public virtual string? Spellcheck
        {
            get => GetAttributeLower("spellcheck");
            set => SetAttributeLower("spellcheck", value);
        }

        /// <summary>
        /// The 'style' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        public virtual string? Style
        {
            get => GetAttributeLower("style");
            set => SetAttributeLower("style", value);
        }

        /// <summary>
        /// The 'tabindex' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The index of the tab.
        /// </value>
        public virtual string? TabIndex
        {
            get => GetAttributeLower("tabindex");
            set => SetAttributeLower("tabindex", value);
        }

        /// <summary>
        /// The 'title' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public virtual string? Title
        {
            get => GetAttributeLower("title");
            set => SetAttributeLower("title", value);
        }

        /// <summary>
        /// The 'translate' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The translate.
        /// </value>
        public virtual string? Translate
        {
            get => GetAttributeLower("translate");
            set => SetAttributeLower("translate", value);
        }

        #endregion

        #region DOM tree queries

        /// <summary>
        /// Element's child nodes
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public IEnumerable<Node> Children
        {
            get => _children;
            set
            {
                if (_children.Equals(value)) return;
                var list = value.ToArray();
                BeginUpdate();
                ClearChildren();
                AppendChild(list);
                EndUpdate();
            }
        }

        /// <summary>
        /// Returns the number of child nodes.
        /// </summary>
        /// <value>
        /// The child count.
        /// </value>
        public int ChildCount => _children.Count;

        /// <summary>
        /// Gets the child at the given index.
        /// </summary>
        /// <param name="index">The index of the child.</param>
        /// <returns>Child node at the given index</returns>
        public Node GetChildAt(int index) => _children[index];

        /// <summary>
        /// Searches for a direct child node and returns its index in the list of child nodes.
        /// </summary>
        /// <param name="node">The node to search for.</param>
        /// <returns>The 0-based child index, or -1 if not found.</returns>
        public int GetChildNodePosition(Node node)
        {
            var index = 0;
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

        /// <summary>
        /// Searches for a direct child element and returns its index in the list of child elements.
        /// </summary>
        /// <param name="element">The element to search for.</param>
        /// <returns>The 0-based child index, or -1 if not found.</returns>
        public int GetChildElementPosition(Element element)
        {
            var index = 0;
            foreach (var child in _children)
            {
                if (child == element)
                {
                    return index;
                }

                if (child is Element)
                {
                    index++;
                }
            }
            return -1;
        }

        /// <summary>
        /// Verifies whether an element descends from another.
        /// </summary>
        /// <param name="element">The element that may be a parent.</param>
        /// <returns>True if the current element descends from the given element.</returns>
        public bool DescendsFrom(Element? element)
        {
            if (this == element)
            {
                return true;
            }

            if (element == null || ParentElement == null)
            {
                return false;
            }
            return ParentElement == element || ParentElement.DescendsFrom(element);
        }

        /// <summary>
        /// Determines whether the node is a direct child.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///   <c>true</c> if the specified node is a child; otherwise, <c>false</c>.
        /// </returns>
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

        /// <summary>
        /// Appends a child node.
        /// </summary>
        /// <param name="nodes">The node to append.</param>
        public void AppendChild(params Node[] nodes)
        {
            BeginUpdate();
            var append = new DomSurgeon(this);
            foreach (var node in nodes)
            {
                append.Append(node);
                OnChildAdded(node);
            }
            EndUpdate();
        }

        /// <summary>
        /// Appends text inside an element.
        /// When the element's last child is a text node, the text is appended to that node.
        /// Otherwise, a new child text node is added to the element.
        /// </summary>
        /// <param name="text">Text of the node</param>
        public void AppendText(string text)
        {
            AppendEncode(text, true);
        }

        /// <summary>
        /// Appends raw HTML inside an element. The HTML won't be verified or parsed by Lara.
        /// </summary>
        /// <param name="data">raw HTML</param>
        public void AppendData(string data)
        {
            AppendEncode(data, false);
        }

        internal void AppendEncode(string? data, bool encode)
        {
            var count = _children.Count;
            if (count > 0 && _children[count - 1] is TextNode node)
            {
                node.AppendEncode(data, encode);
            }
            else
            {
                AppendChild(new TextNode(data, encode));
            }
        }

        /// <summary>
        /// Inserts a child node, right before the specified node.
        /// </summary>
        /// <param name="before">The node that is before.</param>
        /// <param name="node">The node to insert.</param>
        public void InsertChildBefore(Node before, Node node)
        {
            var append = new DomSurgeon(this);
            append.InsertChildBefore(before, node);
            OnChildAdded(node);
        }

        /// <summary>
        /// Inserts a child node, right after the specified node.
        /// </summary>
        /// <param name="after">The node that is after.</param>
        /// <param name="node">The node to insert.</param>
        public void InsertChildAfter(Node after, Node node)
        {
            var append = new DomSurgeon(this);
            append.InsertChildAfter(after, node);
            OnChildAdded(node);
        }

        /// <summary>
        /// Inserts a node at a given index position
        /// </summary>
        /// <param name="index">0-based index position</param>
        /// <param name="node">Node to insert</param>
        public void InsertChildAt(int index, Node node)
        {
            var append = new DomSurgeon(this);
            append.InsertChildAt(index, node);
            OnChildAdded(node);
        }

        /// <summary>
        /// Removes a child.
        /// </summary>
        /// <param name="child">The child.</param>
        public void RemoveChild(Node child)
        {
            var remover = new DomSurgeon(this);
            remover.Remove(child);
        }

        /// <summary>
        /// Removes the child at the given index position
        /// </summary>
        /// <param name="index">Index position</param>
        public void RemoveAt(int index)
        {
            var remover = new DomSurgeon(this);
            remover.RemoveAt(index);
        }

        /// <summary>
        /// Removes all child nodes.
        /// </summary>
        public void ClearChildren()
        {
            var remover = new DomSurgeon(this);
            remover.ClearChildren();
            OnPropertyChanged(nameof(Children));
        }

        /// <summary>
        /// Removes this node from its parent.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot remove from parent, the node has no parent element already</exception>
        public void Remove()
        {
            if (ParentElement == null)
            {
                throw new InvalidOperationException(Resources.CannotRemoveNoParent);
            }
            ParentElement.RemoveChild(this);
        }

        /// <summary>
        /// Swaps two child nodes within the element
        /// </summary>
        /// <param name="index1">Index of 1st node</param>
        /// <param name="index2">Index of 2nd node</param>
        public void SwapChildren(int index1, int index2)
        {
            if (index1 == index2)
            {
                return;
            }
            var temp = _children[index1];
            _children[index1] = _children[index2];
            _children[index2] = temp;
            SwapChildrenDelta.Enqueue(this, index1, index2);
        }

        internal virtual void NotifyValue(ElementEventValue entry)
        {
        }

        private protected virtual void OnChildAdded(Node child)
        {
            OnPropertyChanged(nameof(Children));
        }

        /// <summary>
        /// Occurs when the element is connected to the document's DOM.
        /// </summary>
        protected virtual void OnConnect()
        {
        }

        /// <summary>
        /// Occurs when the element is disconnected from the document's DOM.
        /// </summary>
        protected virtual void OnDisconnect()
        {
        }

        /// <summary>
        /// Occurs when the element is moved from one document to another.
        /// </summary>
        protected virtual void OnAdopted()
        {
        }

        /// <summary>
        /// Occurs when the element or one of its containing elements is moved within the same document's DOM.
        /// </summary>
        protected virtual void OnMove()
        {
        }

        internal void NotifyConnect()
        {
            FlushEvents();
            OnConnect();
            foreach (var child in GetNotifyList())
            {
                child.NotifyConnect();
            }
        }

        internal void NotifyDisconnect()
        {
            OnDisconnect();
            foreach (var child in GetNotifyList())
            {
                child.NotifyDisconnect();
            }
        }

        internal void NotifyAdopted()
        {
            OnAdopted();
            foreach (var child in GetNotifyList())
            {
                child.NotifyAdopted();
            }
        }

        internal void NotifyMove()
        {
            OnMove();
            foreach (var child in GetNotifyList())
            {
                child.NotifyMove();
            }
        }

        internal virtual IEnumerable<Element> GetNotifyList()
        {
            foreach (var node in _children)
            {
                if (node is Element child)
                {
                    yield return child;
                }
            }
        }

        #endregion

        #region Generate Delta content

        internal override ContentNode GetContentNode()
        {
            var list = new List<Node>(GetLightSlotted());
            if (list.Count == 1 && list[0] == this)
            {
                return new ContentElementNode
                {
                    TagName = TagName,
                    NS = GetAttributeLower("xlmns"),
                    Attributes = CopyAttributes(),
                    Children = CopyLightChildren()
                };
            }

            var array = new ContentArrayNode
            {
                Nodes = new List<ContentNode>()
            };
            foreach (var node in list)
            {
                array.Nodes.Add(node.GetContentNode());
            }
            return array;
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

        private List<ContentNode> CopyLightChildren()
        {
            var list = new List<ContentNode>();
            foreach (var child in GetLightChildren())
            {
                list.Add(child.GetContentNode());
            }
            return list;
        }

        #endregion

        #region Subscribe to events

        internal Task NotifyEvent(string eventName)
        {
            if (Events.TryGetValue(eventName, out var settings)
                && settings.Handler != null)
            {
                return settings.Handler();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers an event and associates code to execute.
        /// </summary>
        /// <param name="settings">The event's settings.</param>
        public void On(EventSettings settings)
        {
            settings = settings ?? throw new ArgumentNullException(nameof(settings));
            settings.Verify();
            RemoveEvent(settings.EventName);
            Events.Add(settings.EventName, settings);
            if (Document != null)
            {
                SubscribeDelta.Enqueue(this, settings);
            }
        }

        /// <summary>
        /// Registers an event and associates code to execute.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler to execute.</param>
        public void On(string eventName, Func<Task>? handler)
        {
            eventName = eventName ?? throw new ArgumentNullException(nameof(eventName));
            if (handler == null)
            {
                RemoveEvent(eventName);
            }
            else
            {
                On(new EventSettings
                {
                    EventName = eventName,
                    Handler = handler
                });
            }
        }

        /// <summary>
        /// Registers an event and associates code to execute.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler to execute.</param>
        public void On(string eventName, Action? handler)
        {
            eventName = eventName ?? throw new ArgumentNullException(nameof(eventName));
            if (handler == null)
            {
                RemoveEvent(eventName);
            }
            else
            {
                On(new EventSettings
                {
                    EventName = eventName,
                    Handler = () =>
                    {
                        handler();
                        return Task.CompletedTask;
                    }
                });
            }
        }

        private void RemoveEvent(string eventName)
        {
            if (!Events.ContainsKey(eventName)) return;
            Events.Remove(eventName);
            Document?.Enqueue(new UnsubscribeDelta
            {
                ElementId = Id,
                EventName = eventName
            });
        }

        private void FlushEvents()
        {
            foreach (var settings in Events.Values)
            {
                SubscribeDelta.Enqueue(this, settings);
            }
        }

        #endregion

        #region Binding

        private HashSet<BindingSubscription>? _subscriptions;

        private ChildrenBindingSubscription? _childrenBinding;

        private int _applyingBinding;
        private const int MaxApplyLevel = 5;

        internal void AddSubscription(INotifyPropertyChanged source, Action action)
        {
            _subscriptions ??= new HashSet<BindingSubscription>();
            action();
            _subscriptions.Add(new BindingSubscription(source, (_, _) =>
            {
                if (_applyingBinding > MaxApplyLevel)
                {
                    throw new InvalidOperationException("Cycle detected when applying updates on bindings");
                }
                _applyingBinding++;
                action();
                _applyingBinding--;
            }));
        }

        internal void SubscribeChildren(
            INotifyCollectionChanged source,
            NotifyCollectionChangedEventHandler handler)
        {
            _childrenBinding?.Unsubscribe();
            _childrenBinding = new ChildrenBindingSubscription(handler, source);
        }

        private void ClearSubscriptions()
        {
            _childrenBinding?.Unsubscribe();
            if (_subscriptions == null) return;
            foreach (var item in _subscriptions)
            {
                item.Unsubscribe();
            }
            _subscriptions.Clear();
        }

        /// <summary>
        /// Removes all bindings for the element
        /// </summary>
        public void UnbindAll()
        {
            ClearSubscriptions();
        }

        /// <summary>
        /// Clears all child nodes and replaces them with raw HTML code. The HTML won't be parsed by Lara.
        /// </summary>
        /// <param name="data">raw HTML</param>
        public void SetInnerData(string data)
        {
            SetInnerEncode(data, false);
        }

        internal void SetInnerEncode(string? value, bool encode)
        {
            if (_children.Count == 1 && _children[0] is TextNode node)
            {
                if (encode)
                {
                    node.SetEncodedText(value);
                }
                else
                {
                    node.Data = value;
                }
            }
            else
            {
                ClearChildren();
                AppendEncode(value, encode);
            }
        }

        internal override string? GetNodeInnerText()
        {
            if (ChildCount == 0)
            {
                return string.Empty;
            }

            if (ChildCount == 1)
            {
                return GetChildAt(0).InnerText;
            }
            var builder = new StringBuilder();
            AppendNodeInnerText(builder);
            return builder.ToString();
        }

        internal override void AppendNodeInnerText(StringBuilder builder)
        {
            foreach (var child in Children)
            {
                child.AppendNodeInnerText(builder);
            }
        }

        internal override void SetNodeInnerText(string? value)
        {
            SetInnerEncode(value, true);
        }

        #endregion

        #region Component-related

        internal virtual IEnumerable<Node> GetLightSlotted()
        {
            yield return this;
        }

        internal IEnumerable<Node> GetLightChildren()
        {
            foreach (var node in Children)
            {
                if (node is Element childElement)
                {
                    foreach (var light in childElement.GetLightSlotted())
                    {
                        yield return light;
                    }
                }
                else
                {
                    yield return node;
                }
            }
        }

        internal virtual IEnumerable<Node> GetAllDescendants()
        {
            return Children;
        }

        internal virtual void AttributeChanged(string attribute, string? value)
        {
            OnPropertyChanged(attribute);
        }

        internal bool TryGetQueue([NotNullWhen(true)] out Document? document)
        {
            return TryGetEvents(out document)
                && Document != null
                && Document.QueueingEvents;
        }

        internal bool TryGetEvents([NotNullWhen(true)] out Document? document)
        {
            document = Document;
            return document != null
                   && IsSlotted
                   && IsPrintable
                   && Document != null;
        }

        #endregion

        #region Other methods

        /// <summary>
        /// Focuses this element.
        /// </summary>
        // ReSharper disable once VirtualMemberNeverOverridden.Global
        public virtual void Focus()
        {
            if (Document == null)
            {
                throw new InvalidOperationException(Resources.FocusDisconnected);
            }
            FocusDelta.Enqueue(this);
        }

        /// <summary>
        /// Calculates and returns the HTML code of the element
        /// </summary>
        /// <returns>HTML code</returns>
        public string GetHtml()
        {
            var writer = new DocumentWriter();
            writer.PrintElement(this, 0);
            return writer.ToString();
        }

        #endregion
    }
}
