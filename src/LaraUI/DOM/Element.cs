/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using Integrative.Lara.DOM;
using Integrative.Lara.Front.Tools;
using Integrative.Lara.Reactive;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

        internal Dictionary<string, Func<Task>> Events { get; }

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
        public static Element CreateNS(string ns, string tagName) => ElementFactory.CreateElementNS(ns, tagName);

        internal Element(string tagName)
            : base()
        {
            _attributes = new Attributes(this);
            _children = new List<Node>();
            Events = new Dictionary<string, Func<Task>>();
            TagName = tagName.ToLowerInvariant();
        }

        /// <summary>
        /// Gets the type of the node.
        /// </summary>
        /// <value>
        /// The type of the node.
        /// </value>
        public override NodeType NodeType => NodeType.Element;

        internal bool NeedsId => GetNeedsId();

        internal bool GetNeedsId()
        {
            if (!string.IsNullOrEmpty(_id))
            {
                return false;
            }
            else if (Events.Count > 0)
            {
                return true;
            }
            else if (HtmlReference.RequiresId(TagName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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
            if (string.IsNullOrEmpty(_id))
            {
                return TagName;
            }
            else
            {
                return $"{TagName} #{_id}{suffix}";
            }
        }

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

        /// <summary>
        /// Returns the element's identifier, generating an ID if currently blank.
        /// </summary>
        /// <returns>Element's ID</returns>
        public string EnsureElementId()
        {
            if (string.IsNullOrEmpty(_id))
            {
                if (Document == null)
                {
                    Id = GlobalSerializer.GenerateElementId();
                }
                else
                {
                    Id = Document.GenerateElementId();
                }
            }
            return Id;
        }

        /// <summary>
        /// Sets an attribute and its value.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <param name="attributeValue">The value of the attribute.</param>
        public void SetAttribute(string attributeName, string attributeValue)
        {
            SetAttributeLower(attributeName.ToLower(), attributeValue);
        }

        internal void SetAttributeLower(string nameLower, string value)
        {
            if (nameLower == "id")
            {
                Id = value;
            }
            else
            {
                _attributes.SetAttributeLower(nameLower, value);
            }
        }

        /// <summary>
        /// Determines whether the element has the given attribute
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        /// <returns>
        ///   <c>true</c> if the element has the specified attribute; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAttribute(string attributeName) => _attributes.HasAttribute(attributeName);

        internal bool HasAttributeLower(string nameLower) => _attributes.HasAttributeLower(nameLower);

        /// <summary>
        /// Gets the value of an attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <returns>Value of the attribute</returns>
        public string GetAttribute(string attributeName) => _attributes.GetAttribute(attributeName);

        internal string GetAttributeLower(string nameLower)
            => _attributes.GetAttributeLower(nameLower);

        /// <summary>
        /// Adds or removes a flag attribute
        /// </summary>
        /// <param name="attributeName">Attribute's name</param>
        /// <param name="value">true to add, false to remove</param>
        public void SetFlagAttribute(string attributeName, bool value)
            => _attributes.SetFlagAttributeLower(attributeName.ToLower(), value);

        internal void SetFlagAttributeLower(string nameLower, bool value)
            => _attributes.SetFlagAttributeLower(nameLower, value);

        /// <summary>
        /// Removes an attribute.
        /// </summary>
        /// <param name="attributeName">The name of the attribute.</param>
        public void RemoveAttribute(string attributeName)
        {
            attributeName = attributeName.ToLower();
            if (attributeName == "id")
            {
                Id = null;
            }
            else
            {
                _attributes.RemoveAttributeLower(attributeName);
            }
        }

        internal int? GetIntAttribute(string nameLower)
        {
            if (int.TryParse(GetAttributeLower(nameLower), out int value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        internal void SetIntAttribute(string nameLower, int? value)
        {
            if (value == null)
            {
                _attributes.RemoveAttributeLower(nameLower);
            }
            else
            {
                string text = ((int)value).ToString(CultureInfo.InvariantCulture);
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
        public void ToggleClass(string className, bool value) => Class = ClassEditor.ToggleClass(Class, className, value);

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
        public string AccessKey
        {
            get => _attributes.GetAttributeLower("accesskey");
            set { _attributes.SetAttributeLower("accesskey", value); }
        }

        /// <summary>
        /// The 'autocapitalize' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The automatic capitalize.
        /// </value>
        public string AutoCapitalize
        {
            get => _attributes.GetAttributeLower("autocapitalize");
            set { _attributes.SetAttributeLower("autocapitalize", value); }
        }

        /// <summary>
        /// The 'class' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The class.
        /// </value>
        public string Class
        {
            get => _attributes.GetAttributeLower("class");
            set { _attributes.SetAttributeLower("class", value); }
        }

        /// <summary>
        /// The 'contenteditable' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The content editable.
        /// </value>
        public string ContentEditable
        {
            get => _attributes.GetAttributeLower("contenteditable");
            set { _attributes.SetAttributeLower("contenteditable", value); }
        }

        /// <summary>
        /// The 'contextmenu' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The context menu.
        /// </value>
        public string ContextMenu
        {
            get => _attributes.GetAttributeLower("contextmenu");
            set { _attributes.SetAttributeLower("contextmenu", value); }
        }

        /// <summary>
        /// The 'dir' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The dir.
        /// </value>
        public string Dir
        {
            get => _attributes.GetAttributeLower("dir");
            set { _attributes.SetAttributeLower("dir", value); }
        }

        /// <summary>
        /// The 'draggable' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The draggable.
        /// </value>
        public string Draggable
        {
            get => _attributes.GetAttributeLower("draggable");
            set { _attributes.SetAttributeLower("draggable", value); }
        }

        /// <summary>
        /// The 'dropzone' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The drop zone.
        /// </value>
        public string DropZone
        {
            get => _attributes.GetAttributeLower("dropzone");
            set { _attributes.SetAttributeLower("dropzone", value); }
        }

        /// <summary>
        /// The 'hidden' HTML5 attribute.
        /// </summary>
        /// <value>
        ///   <c>true</c> if hidden; otherwise, <c>false</c>.
        /// </value>
        public bool Hidden
        {
            get => _attributes.HasAttributeLower("hidden");
            set { _attributes.SetFlagAttributeLower("hidden", value); }
        }

        /// <summary>
        /// The 'inputmode' HTML5 attribute.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [input mode]; otherwise, <c>false</c>.
        /// </value>
        public bool InputMode
        {
            get => _attributes.HasAttributeLower("inputmode");
            set { _attributes.SetFlagAttributeLower("inputmode", value); }
        }

        /// <summary>
        /// The 'lang' HTML5 attribute
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Lang
        {
            get => _attributes.GetAttributeLower("lang");
            set { _attributes.SetAttributeLower("lang", value); }
        }

        /// <summary>
        /// The 'spellcheck' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The spellcheck.
        /// </value>
        public string Spellcheck
        {
            get => _attributes.GetAttributeLower("spellcheck");
            set { _attributes.SetAttributeLower("spellcheck", value); }
        }

        /// <summary>
        /// The 'style' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The style.
        /// </value>
        public string Style
        {
            get => _attributes.GetAttributeLower("style");
            set { _attributes.SetAttributeLower("style", value); }
        }

        /// <summary>
        /// The 'tabindex' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The index of the tab.
        /// </value>
        public string TabIndex
        {
            get => _attributes.GetAttributeLower("tabindex");
            set { _attributes.SetAttributeLower("tabindex", value); }
        }

        /// <summary>
        /// The 'title' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title
        {
            get => _attributes.GetAttributeLower("title");
            set { _attributes.SetAttributeLower("title", value); }
        }

        /// <summary>
        /// The 'translate' HTML5 attribute.
        /// </summary>
        /// <value>
        /// The translate.
        /// </value>
        public string Translate
        {
            get => _attributes.GetAttributeLower("translate");
            set { _attributes.SetAttributeLower("translate", value); }
        }

        #endregion

        #region DOM tree queries

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public IEnumerable<Node> Children => _children;

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

        /// <summary>
        /// Searches for a direct child element and returns its index in the list of child elements.
        /// </summary>
        /// <param name="element">The element to search for.</param>
        /// <returns>The 0-based child index, or -1 if not found.</returns>
        public int GetChildElementPosition(Element element)
        {
            int index = 0;
            foreach (var child in _children)
            {
                if (child == element)
                {
                    return index;
                }
                else if (child is Element)
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
        public bool DescendsFrom(Element element)
        {
            if (this == element)
            {
                return true;
            }
            else if (element == null || ParentElement == null)
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
        /// <param name="node">The node to append.</param>
        public void AppendChild(Node node)
        {
            var append = new DomSurgeon(this);
            append.Append(node);
            OnChildAdded(node);
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

        internal void AppendEncode(string data, bool encode)
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
        }

        /// <summary>
        /// Removes this node from its parent.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot remove from parent, the node has no parent element already</exception>
        public void Remove()
        {
            if (ParentElement == null)
            {
                throw new InvalidOperationException("Cannot remove from parent, the node has no parent element already");
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

        internal virtual void OnChildAdded(Node child)
        {
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
            else
            {
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
            if (Events.TryGetValue(eventName, out var handler))
            {
                return handler();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers an event and associates code to execute.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler to execute.</param>
        public void On(string eventName, Func<Task> handler)
        {
            EventWriter.On(this, eventName, handler);
        }

        /// <summary>
        /// Registers an event and associates code to execute.
        /// </summary>
        /// <param name="settings">The event's settings.</param>
        public void On(EventSettings settings)
        {
            EventWriter.On(this, settings);
        }

        #endregion

        #region Binding

        private ElementBindings _bindings;

        private void EnsureBindings()
        {
            if (_bindings == null)
            {
                _bindings = new ElementBindings(this);
            }
        }

        /// <summary>
        /// Binds an element to an action to be triggered whenever the source data changes
        /// </summary>
        /// <typeparam name="T">Type of the source data</typeparam>
        /// <param name="instance">Source data instance</param>
        /// <param name="handler">Action to execute when source data is modified</param>
        public void Bind<T>(T instance, Action<T, Element> handler)
            where T : INotifyPropertyChanged
        {
            EnsureBindings();
            _bindings.BindHandler(new BindHandlerOptions<T>
            {
                Object = instance,
                ModifiedHandler = handler
            });
        }

        /// <summary>
        /// Binds an element to an action to be triggered whenever the source data changes
        /// </summary>
        /// <typeparam name="T">Type of the source data</typeparam>
        /// <param name="options">Binding options</param>
        public void Bind<T>(BindHandlerOptions<T> options)
            where T : INotifyPropertyChanged
        {
            EnsureBindings();
            _bindings.BindHandler(options);
        }

        /// <summary>
        /// Binds an attribute
        /// </summary>
        /// <typeparam name="T">Data type for data source instance</typeparam>
        /// <param name="options">Attribute binding options</param>
        public void BindAttribute<T>(BindAttributeOptions<T> options)
            where T : INotifyPropertyChanged
        {
            EnsureBindings();
            _bindings.BindAttribute<T>(options);
        }

        /// <summary>
        /// Binds a flag attribute
        /// </summary>
        /// <typeparam name="T">Data type for data source instance</typeparam>
        /// <param name="options">Binding options</param>
        public void BindFlagAttribute<T>(BindFlagAttributeOptions<T> options)
            where T : INotifyPropertyChanged
        {
            EnsureBindings();
            _bindings.BindFlagAttribute<T>(options);
        }

        /// <summary>
        /// Bindings to toggle an element class
        /// </summary>
        /// <typeparam name="T">Data type for data source instance</typeparam>
        /// <param name="options">Binding options</param>
        public void BindToggleClass<T>(BindToggleClassOptions<T> options)
            where T : INotifyPropertyChanged
        {
            EnsureBindings();
            _bindings.BindToggleClass<T>(options);
        }

        /// <summary>
        /// Removes bindings for an attribute
        /// </summary>
        /// <param name="attribute">Attribute to remove bindings of</param>
        public void UnbindAttribute(string attribute)
        {
            _bindings?.UnbindAttribute(attribute);
        }

        /// <summary>
        /// Binds an element's inner text
        /// </summary>
        /// <typeparam name="T">Type of source data</typeparam>
        /// <param name="options">Inner text binding options</param>
        public void BindInnerText<T>(BindInnerTextOptions<T> options)
            where T : INotifyPropertyChanged
        {
            EnsureBindings();
            _bindings.BindInnerText(options);
        }

        /// <summary>
        /// Removes inner text bindings
        /// </summary>
        public void UnbindInnerText()
        {
            _bindings?.UnbindInnerText();
        }

        /// <summary>
        /// Removes bindings for the generic handler
        /// </summary>
        public void UnbindHandler()
        {
            _bindings?.UnbindHandler();
        }

        /// <summary>
        /// Removes bindings for any attributes
        /// </summary>
        public void UnbindAttributes()
        {
            _bindings?.UnbindAllAttributes();
        }

        /// <summary>
        /// Binds the list of children to an observable collection
        /// </summary>
        /// <typeparam name="T">Type for items in the collection</typeparam>
        /// <param name="options">Children binding options</param>
        public void BindChildren<T>(BindChildrenOptions<T> options)
            where T : INotifyPropertyChanged
        {
            EnsureBindings();
            _bindings.BindChildren(options);
        }

        /// <summary>
        /// Removes all bindings for the list of children
        /// </summary>
        public void UnbindChildren()
        {
            _bindings?.UnbindChildren();
        }

        /// <summary>
        /// Removes all bindings for the element
        /// </summary>
        public void UnbindAll()
        {
            _bindings?.UnbindAll();
        }

        /// <summary>
        /// Clears all child nodes and replaces them with a single text node
        /// </summary>
        /// <param name="text">Text for the node</param>
        public void SetInnerText(string text)
        {
            SetInnerEncode(text, true);
        }

        /// <summary>
        /// Clears all child nodes and replaces them with raw HTML code. The HTML won't be parsed by Lara.
        /// </summary>
        /// <param name="data">raw HTML</param>
        public void SetInnerData(string data)
        {
            SetInnerEncode(data, false);
        }

        internal void SetInnerEncode(string value, bool encode)
        {
            if (_children.Count == 1 && _children[0] is TextNode node)
            {
                if (encode)
                {
                    node.Data = value;
                }
                else
                {
                    node.SetEncodedText(value);
                }
            }
            else
            {
                ClearChildren();
                AppendEncode(value, encode);
            }
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

        internal virtual void AttributeChanged(string attribute, string value)
        {
        }

        internal bool QueueOpen =>
            IsSlotted
            && Document != null
            && Document.QueueingEvents;

        internal virtual bool SlottingChild(Node child) => IsSlotted;

        internal virtual bool SlottingAllChildren() => IsSlotted;

        internal override void NotifySlotted()
        {
            base.NotifySlotted();
            foreach (var child in Children)
            {
                child.NotifySlotted();
            }
        }

        #endregion

        #region Other methods

        /// <summary>
        /// Focuses this element.
        /// </summary>
        public void Focus()
        {
            if (Document == null)
            {
                throw new InvalidOperationException("To use the focus() method, first add the element to a document.");
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
