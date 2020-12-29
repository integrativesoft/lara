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
    /// Label element
    /// </summary>
    [Obsolete("Use HtmlLabelElement instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class Label : HtmlLabelElement
    {
    }

    /// <summary>
    /// The 'label' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlLabelElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlLabelElement"/> class.
        /// </summary>
        public HtmlLabelElement() : base("label")
        {
        }

        /// <summary>
        /// Gets or sets the 'for' HTML5 attribute.
        /// </summary>
        public string? For
        {
            get => GetAttributeLower("for");
            set => SetAttributeLower("for", value);
        }
    }
}
