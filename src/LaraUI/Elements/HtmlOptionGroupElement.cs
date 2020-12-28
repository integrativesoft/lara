/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;

namespace Integrative.Lara
{
    /// <summary>
    /// Option group
    /// </summary>
    [Obsolete("Use HtmlOptionGroupElement instead")]
    public class OptionGroup : HtmlOptionGroupElement
    {
    }

    /// <summary>
    /// The 'optgroup' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlOptionGroupElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlOptionGroupElement"/> class.
        /// </summary>
        public HtmlOptionGroupElement() : base("optgroup")
        {
        }

        /// <summary>
        /// Gets or sets the 'disabled' HTML5 attribute.
        /// </summary>
        public bool Disabled
        {
            get => HasAttributeLower("disabled");
            set => SetFlagAttributeLower("disabled", value);
        }

        /// <summary>
        /// Gets or sets the 'label' HTML5 attribute.
        /// </summary>
        public string? Label
        {
            get => GetAttributeLower("label");
            set => SetAttributeLower("label", value);
        }

        /// <summary>
        /// Gets the child options.
        /// </summary>
        public IEnumerable<HtmlOptionElement> Options => GetOptions();

        private IEnumerable<HtmlOptionElement> GetOptions()
        {
            foreach (var node in Children)
            {
                if (node is HtmlOptionElement option)
                {
                    yield return option;
                }
            }
        }

        private protected override void OnChildAdded(Node child)
        {
            BeginUpdate();
            base.OnChildAdded(child);
            if (ParentElement is HtmlSelectElement parent
                && child is HtmlOptionElement option
                && !string.IsNullOrEmpty(parent.Value))
            {
                option.NotifyAdded(parent.Value);
            }
            EndUpdate();
        }

        internal void NotifyAdded(string parentValue)
        {
            foreach (var child in GetOptions())
            {
                if (child.Value == parentValue)
                {
                    child.Selected = true;
                }
            }
        }
    }
}
