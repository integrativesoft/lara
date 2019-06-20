﻿/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;

namespace Integrative.Lara
{
    /// <summary>
    /// The 'optgroup' HTML5 element.
    /// </summary>
    /// <seealso cref="Integrative.Lara.Element" />
    public sealed class OptionGroup : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionGroup"/> class.
        /// </summary>
        public OptionGroup() : base("optgroup")
        {
        }

        /// <summary>
        /// Gets or sets the 'disabled' HTML5 attribute.
        /// </summary>
        public bool Disabled
        {
            get => HasAttributeLower("disabled");
            set { SetFlagAttributeLower("disabled", value); }
        }

        /// <summary>
        /// Gets or sets the 'label' HTML5 attribute.
        /// </summary>
        public string Label
        {
            get => GetAttributeLower("label");
            set { SetAttributeLower("label", value); }
        }

        /// <summary>
        /// Gets the child options.
        /// </summary>
        public IEnumerable<Option> Options => GetOptions();

        private IEnumerable<Option> GetOptions()
        {
            foreach (var node in Children)
            {
                if (node is Option option)
                {
                    yield return option;
                }
            }
        }

        internal override void OnChildAdded(Node child)
        {
            if (ParentElement is Select parent && child is Option option)
            {
                var value = parent.Value;
                if (!string.IsNullOrEmpty(value))
                {
                    option.NotifyAdded(value);
                }
            }
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
