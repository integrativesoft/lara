/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Integrative.Lara
{
    /// <summary>
    /// Base class for web components
    /// </summary>
    public abstract class WebComponent : Element
    {
        /// <summary>
        /// The 'shadow root' is the element that contains the shadow DOM tree
        /// </summary>
        protected Element ShadowRoot => _shadow;

        private readonly Shadow _shadow;

        internal Shadow GetShadow() => _shadow;

        private HashSet<string>? _observedAttributes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tagName">Component's custom tag name</param>
        protected WebComponent(string tagName) : base(tagName ?? throw new ArgumentNullException(nameof(tagName)))
        {
            VerifyTypeThrow(tagName, GetType());
            _shadow = new Shadow(this);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        protected WebComponent()
        {
            VerifyTypeThrow(TagName, GetType());
            _shadow = new Shadow(this);
        }

        private void InitializeObservedAttributes()
        {
            if (_observedAttributes == null)
            {
                _observedAttributes = new HashSet<string>(GetObservedAttributes());
            }
        }

        private static void VerifyTypeThrow(string tagName, Type componentType)
        {
            if (!VerifyType(tagName, componentType, out var error))
            {
                throw new InvalidOperationException(error);
            }
        }

        internal static bool VerifyType(string tagName, Type componentType, out string error)
        {
            // register component if not previous;y registered
            var app = LaraUI.Context.Application;
            if (!app.TryGetComponent(tagName, out var type))
            {
                app.PublishComponent(new WebComponentOptions
                {
                    ComponentTagName = tagName,
                    ComponentType = componentType
                });
                error = "";
                return true;
            }

            // error if already registered for different type
            if (type != componentType)
            {
                error = $"The tag '{tagName}' is registered with the type '{type.FullName}' and not '{componentType.FullName}'.";
                return false;
            }

            // already registered with matching type
            error = string.Empty;
            return true;
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        [Obsolete("Not needed anymore, Shadow root is automatically created")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void AttachShadow()
        {
        }

        /// <summary>
        /// Override to declare a list of attributes that will trigger the OnAttributeChanged event
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetObservedAttributes()
        {
            return Enumerable.Empty<string>();
        }

        internal override IEnumerable<Node> GetLightSlotted()
        {
            foreach (var child in ShadowRoot.Children)
            {
                if (child is Element childElement)
                {
                    foreach (var light in childElement.GetLightSlotted())
                    {
                        yield return light;
                    }
                }
                else
                {
                    yield return child;
                }
            }
        }

        internal override IEnumerable<Node> GetAllDescendants()
        {
            yield return ShadowRoot;
            foreach (var child in Children)
            {
                yield return child;
            }
        }

        /// <summary>
        /// Returns the elements that are slotted with the given slot name
        /// </summary>
        /// <param name="slotName">Slot name</param>
        /// <returns>IEnumerable of nodes</returns>
        public IEnumerable<Node> GetSlottedElements(string? slotName)
        {
            foreach (var node in Children)
            {
                if (NodeMatchesSlot(node, slotName))
                {
                    yield return node;
                }
            }
        }

        private static bool NodeMatchesSlot(Node node, string? slotName)
        {
            return node is Element element
                && ElementMatchesSlot(element, slotName);
        }

        private static bool ElementMatchesSlot(Element element, string? slotName)
        {
            var slot = element.GetAttributeLower("slot");
            if (string.IsNullOrEmpty(slotName))
            {
                return string.IsNullOrEmpty(slot);
            }

            return slot == slotName;
        }

        internal override void AttributeChanged(string attribute, string? value)
        {
            BeginUpdate();
            base.AttributeChanged(attribute, value);
            InitializeObservedAttributes();
            if (_observedAttributes != null && _observedAttributes.Contains(attribute))
            {
                OnAttributeChanged(attribute);
            }
            EndUpdate();
        }

        /// <summary>
        /// Invoked each time an attribute defined in GetObservedAttributes is modified.
        /// </summary>
        /// <param name="attribute"></param>
        protected virtual void OnAttributeChanged(string attribute)
        {
        }

        internal override IEnumerable<Element> GetNotifyList()
        {
            foreach (var child in base.GetNotifyList())
            {
                yield return child;
            }
            yield return ShadowRoot;
        }

        internal override bool IsPrintable => false;

        internal bool IsSlotActive(string? slotName)
        {
            return IsSlotActive(ShadowRoot, slotName);
        }

        private static bool IsSlotActive(Element parent, string? slotName)
        {
            foreach (var child in parent.Children)
            {
                if (IsSlotChildActive(child, slotName))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsSlotChildActive(Node child, string? slotName)
        {
            return (child is Slot slot && slot.MatchesName(slotName))
                || (child is Element element && IsSlotActive(element, slotName));
        }

        /// <summary>
        /// Triggers a custom event
        /// </summary>
        /// <param name="eventName">Event's name</param>
        public void TriggerEvent(string eventName) => NotifyEvent(eventName);
    }
}
