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
    /// A slot element is a placeholder inside a web component that you can fill with your own element
    /// </summary>
    public sealed class Slot : Element
    {
        /// <summary>
        /// The slot's name
        /// </summary>
        public string Name
        {
            get => GetAttribute("name");
            set { SetAttributeLower("name", value); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Slot() : base("slot")
        {
        }

        internal override IEnumerable<Node> GetLightSlotted()
        {
            if (TryFindParentComponent(this, out var component))
            {
                return component.GetSlottedElements(Name);
            }
            else
            {
                return Enumerable.Repeat(this, 1);
            }
        }

        private static bool TryFindParentComponent(Element element, out WebComponent component)
        {
            var parent = element.ParentElement;
            if (parent is null)
            {
                component = default;
                return false;
            }
            else if (parent is Shadow shadow)
            {
                component = shadow.ParentComponent;
                return true;
            }
            else
            {
                return TryFindParentComponent(parent, out component);
            }
        }

        internal override bool SlottingAllChildren() => false;

        internal override bool SlottingChild(Node child) => false;

        internal override void NotifySlotted()
        {
        }
    }
}
