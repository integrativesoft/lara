/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.ComponentModel;

namespace Integrative.Lara
{
    /// <summary>
    /// ColGroup element
    /// </summary>
    [Obsolete("Use HtmlColGroupElement instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ColGroup : HtmlColGroupElement
    {
    }

    /// <summary>
    /// The 'colgroup' HTML5 element
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlColGroupElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlColGroupElement"/> class.
        /// </summary>
        public HtmlColGroupElement() : base("colgroup")
        {
        }

        /// <summary>
        /// Gets or sets the 'span' HTML5 attribute.
        /// </summary>
        public int? Span
        {
            get => GetIntAttribute("span");
            set => SetIntAttribute("span", value);
        }
    }
}
