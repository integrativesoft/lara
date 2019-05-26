/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class Link : Element
    {
        public Link() : base("link")
        {
        }

        public string CrossOrigin
        {
            get => GetAttributeLower("crossorigin");
            set { SetAttributeLower("crossorigin", value); }
        }

        public string HRef
        {
            get => GetAttributeLower("href");
            set { SetAttributeLower("href", value); }
        }

        public string HRefLang
        {
            get => GetAttributeLower("hreflang");
            set { SetAttributeLower("hreflang", value); }
        }

        public string Media
        {
            get => GetAttributeLower("media");
            set { SetAttributeLower("media", value); }
        }

        public string Rel
        {
            get => GetAttributeLower("rel");
            set { SetAttributeLower("rel", value); }
        }

        public string Sizes
        {
            get => GetAttributeLower("sizes");
            set { SetAttributeLower("sizes", value); }
        }

        public string Type
        {
            get => GetAttributeLower("type");
            set { SetAttributeLower("type", value); }
        }
    }
}
