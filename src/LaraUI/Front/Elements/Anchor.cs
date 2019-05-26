/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class Anchor : Element
    {
        public Anchor() : base("a")
        {
        }

        public bool Download
        {
            get => HasAttribute("download");
            set { SetFlagAttributeLower("download", value); }
        }

        public string HRef
        {
            get => GetAttribute("href");
            set { SetAttributeLower("href", value); }
        }

        public string HRefLang
        {
            get => GetAttribute("hreflang");
            set { SetAttributeLower("hreflang", value); }
        }

        public string Media
        {
            get => GetAttribute("media");
            set { SetAttributeLower("media", value); }
        }

        public string Ping
        {
            get => GetAttribute("ping");
            set { SetAttributeLower("ping", value); }
        }

        public string ReferrerPolicy
        {
            get => GetAttribute("referrerpolicy");
            set { SetAttributeLower("referrerpolicy", value); }
        }

        public string Rel
        {
            get => GetAttribute("rel");
            set { SetAttributeLower("rel", value); }
        }

        public string Target
        {
            get => GetAttributeLower("target");
            set { SetAttributeLower("target", value); }
        }

        public string Type
        {
            get => GetAttributeLower("type");
            set { SetAttributeLower("type", value); }
        }
    }
}
