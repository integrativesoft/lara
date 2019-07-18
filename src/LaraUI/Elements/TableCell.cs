/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    /// <summary>
    /// The 'td' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public sealed class TableCell : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableCell"/> class.
        /// </summary>
        public TableCell() : base("td")
        {
        }

        /// <summary>
        /// Gets or sets the 'colspan' HTML5 attribute.
        /// </summary>
        public int? ColSpan
        {
            get => GetIntAttribute("colspan");
            set { SetIntAttribute("colspan", value); }
        }

        /// <summary>
        /// Gets or sets the 'headers' HTML5 attribute.
        /// </summary>
        public string Headers
        {
            get => GetAttributeLower("headers");
            set { SetAttributeLower("headers", value); }
        }

        /// <summary>
        /// Gets or sets the 'rowspan' HTML5 attribute.
        /// </summary>
        public int? RowSpan
        {
            get => GetIntAttribute("rowspan");
            set { SetIntAttribute("rowspan", value); }
        }
    }
}
