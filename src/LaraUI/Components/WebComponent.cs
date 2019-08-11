/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Components;
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

        private readonly HashSet<string> _observedAttributes;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tagName">Component's custom tag name</param>
        protected WebComponent(string tagName) : base(tagName)
        {
            _observedAttributes = new HashSet<string>(GetObservedAttributes());
        }

        /// <summary>
        /// Creates a shadow DOM tree for this element
        /// </summary>
        protected void AttachShadow()
        {
            ShadowRoot = new Shadow
            {
                ParentComponent = this
            };
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
            if (ShadowRoot == null)
            {
                yield return this;
                yield break;
            }
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

        internal IEnumerable<Node> GetSlotElements(string slotName)
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

        internal override void AttributeChanged(string attribute)
        {
            base.AttributeChanged(attribute);
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
    }
}
