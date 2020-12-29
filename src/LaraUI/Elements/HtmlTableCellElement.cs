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
    /// Table cell element
    /// </summary>
    [Obsolete("Use HtmlTableCellElement instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class TableCell : HtmlTableCellElement
    {
    }

    /// <summary>
    /// The 'td' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlTableCellElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTableCellElement"/> class.
        /// </summary>
        public HtmlTableCellElement() : base("td")
        {
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
    }
}
