/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class Image : Element
    {
        public Image() : base("img")
        {
        }

        public string Alt
        {
            get => GetAttributeLower("alt");
            set { SetAttributeLower("alt", value); }
        }

        public string CrossOrigin
        {
            get => GetAttributeLower("crossorigin");
            set { SetAttributeLower("crossorigin", value); }
        }

        public int? Height
        {
            get => GetIntAttribute("height");
            set { SetIntAttribute("height", value); }
        }

        public bool IsMap
        {
            get => HasAttribute("ismap");
            set { SetFlagAttributeLower("ismap", value); }
        }

        public string LongDesc
        {
            get => GetAttributeLower("longdesc");
            set { SetAttributeLower("longdesc", value); }
        }

        public string Src
        {
            get => GetAttributeLower("src");
            set { SetAttributeLower("src", value); }
        }

        public string SrcSet
        {
            get => GetAttributeLower("srcset");
            set { SetAttributeLower("srcset", value); }
        }

        public string UseMap
        {
            get => GetAttributeLower("usemap");
            set { SetAttributeLower("usemap", value); }
        }

        public int? Width
        {
            get => GetIntAttribute("width");
            set { SetIntAttribute("width", value); }
        }
    }
}
