/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Option element
    /// </summary>
    [Obsolete("Use HtmlOptionElement instead")]
    public class OptionElement : HtmlOptionElement
    {
    }

    /// <summary>
    /// The 'option' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlOptionElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlOptionElement"/> class.
        /// </summary>
        public HtmlOptionElement() : base("option")
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items"></param>
        public HtmlOptionElement(params Node[] items) : base("option", items)
        {
        }

        internal override void NotifyValue(ElementEventValue entry)
        {
            base.NotifyValue(entry);
            NotifySelected(entry.Checked);
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
        /// Gets or sets the 'selected' HTML5 attribute.
        /// </summary>
        public bool Selected
        {
            get => HasAttributeLower("selected");
            set => SetFlagAttributeLower("selected", value);
        }

        /// <summary>
        /// Gets or sets the 'value' HTML5 attribute.
        /// </summary>
        public string? Value
        {
            get => GetAttributeLower("value");
            set => SetAttributeLower("value", value);
        }

        internal void NotifyAdded(string parentValue)
        {
            if (parentValue == Value)
            {
                Selected = true;
            }
        }
    }
}
