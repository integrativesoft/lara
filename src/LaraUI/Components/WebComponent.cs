/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Components;
using System;
using System.Collections.Generic;
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
        protected Element ShadowRoot { get; private set; }

        private HashSet<string> _observedAttributes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tagName">Component's custom tag name</param>
        protected WebComponent(string tagName) : base(tagName)
        {
            tagName = tagName ?? throw new ArgumentNullException(nameof(tagName));
            VerifyTypeThrow(tagName, GetType());
            ShadowRoot = new Shadow
            {
                ParentComponent = this
            };
        }

        private void InitializeObservedAttributes()
        {
            if (_observedAttributes == null)
            {
                _observedAttributes = new HashSet<string>(GetObservedAttributes());
            }
        }

        internal static void VerifyTypeThrow(string tagName, Type componentType)
        {
            if (!VerifyType(tagName, componentType, out string error))
            {
                throw new InvalidOperationException(error);
            }
        }

        internal static bool VerifyType(string tagName, Type componentType, out string error)
        {
            if (!LaraUI.TryGetComponent(tagName, out var type))
            {
                error = $"The tag '{tagName}' is not registered as web component. To register a webcomponent, either (1) decorate it with [LaraWebComponent] and run LaraUI.PublishAssemblies(), or (2) use LaraUI.Publish()."; ;
                return false;
            }
            else if (type != componentType)
            {
                error = $"The tag '{tagName}' is registered with the type '{type.FullName}' and not '{componentType.FullName}'.";
                return false;
            }
            else
            {
                error = string.Empty;
                return true;
            }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        [Obsolete("Not needed anymore, Shadow root is automatically created")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Deprecated")]
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
        public IEnumerable<Node> GetSlottedElements(string slotName)
        {
            foreach (var node in Children)
            {
                if (NodeMatchesSlot(node, slotName))
                {
                    yield return node;
                }
            }
        }

        private static bool NodeMatchesSlot(Node node, string slotName)
        {
            return node is Element element
                && ElementMatchesSlot(element, slotName);
        }

        private static bool ElementMatchesSlot(Element element, string slotName)
        {
            var slot = element.GetAttributeLower("slot");
            if (string.IsNullOrEmpty(slotName))
            {
                return string.IsNullOrEmpty(slot);
            }
            else
            {
                return slot == slotName;
            }
        }

        internal override void AttributeChanged(string attribute, string value)
        {
            base.AttributeChanged(attribute, value);
            InitializeObservedAttributes();
            if (_observedAttributes.Contains(attribute))
            {
                OnAttributeChanged(attribute);
            }
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

        internal override bool SlottingAllChildren() => false;

        internal override bool SlottingChild(Node child)
        {
            return child is Element element
                && SlotNamePresent(element.GetAttributeLower("slot"));
        }

        private bool SlotNamePresent(string slotName)
        {
            foreach (var child in Children)
            {
                if (NodeMatchesSlot(child, slotName))
                {
                    return true;
                }
            }
            return false;
        }

        internal override void NotifySlotted()
        {
            foreach (var child in Children)
            {
                child.NotifySlotted();
            }
            ShadowRoot.NotifySlotted();
        }
    }
}
