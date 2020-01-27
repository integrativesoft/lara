/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    /// <summary>
    /// The HTML5 'a' element
    /// </summary>
    /// <seealso cref="Element" />
    public sealed class Anchor : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Anchor"/> class.
        /// </summary>
        public Anchor() : base("a")
        {
        }

        /// <summary>
        /// The 'download' HTML5 attribute.
        /// </summary>
        public bool Download
        {
            get => HasAttribute("download");
            set => SetFlagAttributeLower("download", value);
        }

        /// <summary>
        /// The 'href' HTML5 attribute.
        /// </summary>
        public string? HRef
        {
            get => GetAttribute("href");
            set => SetAttributeLower("href", value);
        }

        /// <summary>
        /// The 'hreflang' HTML5 attribute.
        /// </summary>
        public string? HRefLang
        {
            get => GetAttribute("hreflang");
            set => SetAttributeLower("hreflang", value);
        }

        /// <summary>
        /// The 'media' HTML5 attribute.
        /// </summary>
        public string? Media
        {
            get => GetAttribute("media");
            set => SetAttributeLower("media", value);
        }

        /// <summary>
        /// The 'ping' HTML5 attribute.
        /// </summary>
        public string? Ping
        {
            get => GetAttribute("ping");
            set => SetAttributeLower("ping", value);
        }

        /// <summary>
        /// The 'referrerpolicy' HTML5 attribute.
        /// </summary>
        public string? ReferrerPolicy
        {
            get => GetAttribute("referrerpolicy");
            set => SetAttributeLower("referrerpolicy", value);
        }

        /// <summary>
        /// The 'rel' HTML5 attribute.
        /// </summary>
        public string? Rel
        {
            get => GetAttribute("rel");
            set => SetAttributeLower("rel", value);
        }

        /// <summary>
        /// The 'target' HTML5 attribute.
        /// </summary>
        public string? Target
        {
            get => GetAttributeLower("target");
            set => SetAttributeLower("target", value);
        }

        /// <summary>
        /// The 'type' HTML5 attribute.
        /// </summary>
        public string? Type
        {
            get => GetAttributeLower("type");
            set => SetAttributeLower("type", value);
        }
    }
}
