/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// LI element
    /// </summary>
    [Obsolete("Use HtmlLiElement instead")]
    public class ListItem : HtmlLiElement
    {
    }

    /// <summary>
    /// The 'li' HTML5 element
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlLiElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlLiElement"/> class.
        /// </summary>
        public HtmlLiElement() : base("li")
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items"></param>
        public HtmlLiElement(params Node[] items) : base("li", items)
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
