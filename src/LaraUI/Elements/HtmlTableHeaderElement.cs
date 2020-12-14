/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Table header element
    /// </summary>
    [Obsolete("Use HtmlTableHeaderElement instead")]
    public class TableHeader : HtmlTableHeaderElement
    {
    }

    /// <summary>
    /// The 'th' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlTableHeaderElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTableHeaderElement"/> class.
        /// </summary>
        public HtmlTableHeaderElement() : base("th")
        {
        }

        /// <summary>
        /// Gets or sets the 'abbr' HTML5 attribute.
        /// </summary>
        public string? Abbr
        {
            get => GetAttributeLower("abbr");
            set => SetAttributeLower("abbr", value);
        }

        /// <summary>
        /// Gets or sets the 'colspan' HTML5 attribute.
        /// </summary>
        public int? ColSpan
        {
            get => GetIntAttribute("colspan");
            set => SetIntAttribute("colspan", value);
        }

        /// <summary>
        /// Gets or sets the 'headers' HTML5 attribute.
        /// </summary>
        public string? Headers
        {
            get => GetAttributeLower("headers");
            set => SetAttributeLower("headers", value);
        }

        /// <summary>
        /// Gets or sets the 'rowspan' HTML5 attribute.
        /// </summary>
        public int? RowSpan
        {
            get => GetIntAttribute("rowspan");
            set => SetIntAttribute("rowspan", value);
        }

        /// <summary>
        /// Gets or sets the 'scope' HTML5 attribute.
        /// </summary>
        public string? Scope
        {
            get => GetAttributeLower("scope");
            set => SetAttributeLower("scope", value);
        }

        /// <summary>
        /// Gets or sets the 'sorted' HTML5 attribute.
        /// </summary>
        public string? Sorted
        {
            get => GetAttributeLower("sorted");
            set => SetAttributeLower("sorted", value);
        }
    }
}
