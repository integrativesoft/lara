/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        public string? Name
        {
            get => GetAttribute("name");
            set => SetAttributeLower("name", value);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Slot() : base("slot")
        {
        }

        internal bool MatchesName(string? slotName)
        {
            var name = Name;
            if (string.IsNullOrEmpty(name))
            {
                return string.IsNullOrEmpty(slotName);
            }

            return name == slotName;
        }

        internal override IEnumerable<Node> GetLightSlotted()
        {
            return TryFindParentComponent(this, out var component) ? component.GetSlottedElements(Name) : Enumerable.Repeat(this, 1);
        }

        private static bool TryFindParentComponent(Node element, [NotNullWhen(true)] out WebComponent? component)
        {
            var parent = element.ParentElement;
            if (parent is null)
            {
                component = default;
                return false;
            }

            if (!(parent is Shadow shadow)) return TryFindParentComponent(parent, out component);
            component = shadow.ParentComponent;
            return true;
            // ReSharper disable once TailRecursiveCall
        }

        internal override bool IsPrintable => false;
    }
}
