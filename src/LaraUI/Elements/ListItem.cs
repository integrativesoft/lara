/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    /// <summary>
    /// The 'li' HTML5 element
    /// </summary>
    /// <seealso cref="Element" />
    public sealed class ListItem : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListItem"/> class.
        /// </summary>
        public ListItem() : base("li")
        {
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
