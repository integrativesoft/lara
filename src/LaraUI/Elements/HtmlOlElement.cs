/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.ComponentModel;

namespace Integrative.Lara
{
    /// <summary>
    /// Ordered list
    /// </summary>
    [Obsolete("Use HtmlOlElement instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class OrderedList : HtmlOlElement
    {
    }

    /// <summary>
    /// The 'ol' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlOlElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlOlElement"/> class.
        /// </summary>
        public HtmlOlElement() : base("ol")
        {
        }

        /// <summary>
        /// Gets or sets the 'reversed' HTML5 attribute.
        /// </summary>
        public bool Reversed
        {
            get => HasAttributeLower("reversed");
            set => SetFlagAttributeLower("reversed", value);
        }

        /// <summary>
        /// Gets or sets the 'start' HTML5 attribute.
        /// </summary>
        public int? Start
        {
            get => GetIntAttribute("start");
            set => SetIntAttribute("start", value);
        }

        /// <summary>
        /// Gets or sets the 'type' HTML5 attribute.
        /// </summary>
        public string? Type
        {
            get => GetAttributeLower("type");
            set => SetAttributeLower("type", value);
        }
    }
}
