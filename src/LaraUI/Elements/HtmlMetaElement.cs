/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Meta element
    /// </summary>
    [Obsolete("Use HtmlMetaElement instead")]
    public class Meta : HtmlMetaElement
    {
    }

    /// <summary>
    /// The 'meta' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlMetaElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlMetaElement"/> class.
        /// </summary>
        public HtmlMetaElement() : base("meta")
        {
        }

        /// <summary>
        /// Gets or sets the 'content' HTML5 attribute.
        /// </summary>
        public string? Content
        {
            get => GetAttributeLower("content");
            set => SetAttributeLower("content", value);
        }

        /// <summary>
        /// Gets or sets the 'httpequiv' HTML5 attribute.
        /// </summary>
        public string? HttpEquiv
        {
            get => GetAttributeLower("http-equiv");
            set => SetAttributeLower("http-equiv", value);
        }

        /// <summary>
        /// Gets or sets the 'name' HTML5 attribute.
        /// </summary>
        public string? Name
        {
            get => GetAttributeLower("name");
            set => SetAttributeLower("name", value);
        }
    }
}
