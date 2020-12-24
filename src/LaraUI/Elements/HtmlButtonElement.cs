/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Button element
    /// </summary>
    [Obsolete("Use HtmlButtonElement instead")]
    public class Button : HtmlButtonElement
    {
    }

    /// <summary>
    /// The 'button' HTML5 element
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlButtonElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlButtonElement"/> class.
        /// </summary>
        public HtmlButtonElement() : base("button")
        {
            Type = "button";
        }

        internal override void NotifyValue(ElementEventValue entry)
        {
            base.NotifyValue(entry);
            NotifyValue(entry.Value);
        }

        /// <summary>
        /// Gets or sets the 'autofocus' HTML5 attribute.
        /// </summary>
        public bool AutoFocus
        {
            get => HasAttributeLower("autofocus");
            set => SetFlagAttributeLower("autofocus", value);
        }

        /// <summary>
        /// Gets or sets the 'disabled' HTML5 attribute.
        /// </summary>
        public bool Disabled
        {
            get => HasAttribute("disabled");
            set => SetFlagAttributeLower("disabled", value);
        }

        /// <summary>
        /// Gets or sets the 'name' HTML5 attribute.
        /// </summary>
        public string? Name
        {
            get => GetAttributeLower("name");
            set => SetAttributeLower("name", value);
        }

        /// <summary>
        /// Gets or sets the 'type' HTML5 attribute.
        /// </summary>
        public string? Type
        {
            get => GetAttributeLower("type");
            set => SetAttributeLower("type", value);
        }

        /// <summary>
        /// Gets or sets the 'value' property.
        /// </summary>
        public string? Value
        {
            get => GetAttributeLower("value");
            set => SetAttributeLower("value", value);
        }
    }
}
