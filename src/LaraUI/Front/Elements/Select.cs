/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class Select : Element
    {
        public Select() : base("select")
        {
        }

        public bool Autofocus
        {
            get => HasAttributeLower("autofocus");
            set { SetFlagAttributeLower("autofocus", value); }
        }

        public bool Disabled
        {
            get => HasAttributeLower("disabled");
            set { SetFlagAttributeLower("disabled", value); }
        }

        public bool Multiple
        {
            get => HasAttributeLower("multiple");
            set { SetFlagAttributeLower("multiple", value); }
        }

        public string Name
        {
            get => GetAttributeLower("name");
            set { SetAttributeLower("name", value); }
        }

        public bool Required
        {
            get => HasAttributeLower("required");
            set { SetFlagAttributeLower("required", value); }
        }

        public int? Size
        {
            get => GetIntAttribute("size");
            set { SetIntAttribute("size", value); }
        }

        public string Value
        {
            get => GetAttributeLower("value");
            set { SetAttributeLower("value", value); }
        }
    }
}
