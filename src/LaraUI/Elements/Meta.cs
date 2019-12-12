/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    /// <summary>
    /// The 'meta' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public sealed class Meta : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Meta"/> class.
        /// </summary>
        public Meta() : base("meta")
        {
        }

        /// <summary>
        /// Gets or sets the 'content' HTML5 attribute.
        /// </summary>
        public string? Content
        {
            get => GetAttributeLower("content");
            set { SetAttributeLower("content", value); }
        }

        /// <summary>
        /// Gets or sets the 'httpequiv' HTML5 attribute.
        /// </summary>
        public string? HttpEquiv
        {
            get => GetAttributeLower("http-equiv");
            set { SetAttributeLower("http-equiv", value); }
        }

        /// <summary>
        /// Gets or sets the 'name' HTML5 attribute.
        /// </summary>
        public string? Name
        {
            get => GetAttributeLower("name");
            set { SetAttributeLower("name", value); }
        }
    }
}
