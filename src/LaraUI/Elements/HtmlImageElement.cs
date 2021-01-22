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
    /// Image element
    /// </summary>
    [Obsolete("Use HtmlImageElement instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class Image : HtmlImageElement
    {
    }

    /// <summary>
    /// The 'img' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlImageElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlImageElement"/> class.
        /// </summary>
        public HtmlImageElement() : base("img")
        {
        }

        /// <summary>
        /// Gets or sets the 'alt' HTML5 attribute.
        /// </summary>
        public string? Alt
        {
            get => GetAttributeLower("alt");
            set => SetAttributeLower("alt", value);
        }

        /// <summary>
        /// Gets or sets the 'crossorigin' HTML5 attribute.
        /// </summary>
        public string? CrossOrigin
        {
            get => GetAttributeLower("crossorigin");
            set => SetAttributeLower("crossorigin", value);
        }

        /// <summary>
        /// Gets or sets the 'height' HTML5 attribute.
        /// </summary>
        public string? Height
        {
            get => GetAttribute("height");
            set => SetAttribute("height", value);
        }

        /// <summary>
        /// Gets or sets the 'ismap' HTML5 attribute.
        /// </summary>
        public bool IsMap
        {
            get => HasAttribute("ismap");
            set => SetFlagAttributeLower("ismap", value);
        }

        /// <summary>
        /// Gets or sets the 'longdesc' HTML5 attribute.
        /// </summary>
        public string? LongDesc
        {
            get => GetAttributeLower("longdesc");
            set => SetAttributeLower("longdesc", value);
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
        /// Gets or sets the 'srcset' HTML5 attribute.
        /// </summary>
        public string? SrcSet
        {
            get => GetAttributeLower("srcset");
            set => SetAttributeLower("srcset", value);
        }

        /// <summary>
        /// Gets or sets the 'usemap' HTML5 attribute.
        /// </summary>
        public string? UseMap
        {
            get => GetAttributeLower("usemap");
            set => SetAttributeLower("usemap", value);
        }

        /// <summary>
        /// Gets or sets the 'width' HTML5 attribute.
        /// </summary>
        public string? Width
        {
            get => GetAttribute("width");
            set => SetAttribute("width", value);
        }
    }
}
