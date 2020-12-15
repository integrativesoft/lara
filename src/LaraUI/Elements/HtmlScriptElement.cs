/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Script element
    /// </summary>
    [Obsolete("Use HtmlScriptElement instead")]
    public class Script : HtmlScriptElement
    {
    }

    /// <summary>
    /// The 'script' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlScriptElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlScriptElement"/> class.
        /// </summary>
        public HtmlScriptElement() : base("script")
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items"></param>
        public HtmlScriptElement(params Node[] items) : base("script", items)
        {
        }

        /// <summary>
        /// Gets or sets the 'async' HTML5 attribute.
        /// </summary>
        public bool Async
        {
            get => HasAttributeLower("async");
            set => SetFlagAttributeLower("async", value);
        }

        /// <summary>
        /// Gets or sets the 'charset' HTML5 attribute.
        /// </summary>
        public string? Charset
        {
            get => GetAttributeLower("charset");
            set => SetAttributeLower("charset", value);
        }

        /// <summary>
        /// Gets or sets the 'defer' HTML5 attribute.
        /// </summary>
        public bool Defer
        {
            get => HasAttributeLower("defer");
            set => SetFlagAttributeLower("defer", value);
        }

        /// <summary>
        /// Gets or sets the 'src' HTML5 attribute.
        /// </summary>
        public string? Src
        {
            get => GetAttributeLower("src");
            set => SetAttributeLower("src", value);
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
