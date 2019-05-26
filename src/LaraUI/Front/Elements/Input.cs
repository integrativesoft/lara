/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class Input : Element
    {
        public Input() : base("input")
        {
        }

        public string Accept
        {
            get => GetAttributeLower("accept");
            set { SetAttributeLower("accept", value); }
        }

        public string Alt
        {
            get => GetAttributeLower("alt");
            set { SetAttributeLower("alt", value); }
        }

        public string Autocomplete
        {
            get => GetAttributeLower("autocomplete");
            set { SetAttributeLower("autocomplete", value); }
        }

        public bool Autofocus
        {
            get => HasAttributeLower("autofocus");
            set { SetFlagAttributeLower("autofocus", value); }
        }

        public bool Checked
        {
            get => HasAttributeLower("checked");
            set { SetFlagAttributeLower("checked", value); }
        }

        public string Dirname
        {
            get => GetAttributeLower("dirname");
            set { SetAttributeLower("dirname", value); }
        }

        public bool Disabled
        {
            get => HasAttributeLower("disabled");
            set { SetFlagAttributeLower("disabled", value); }
        }

        public int? Height
        {
            get => GetIntAttribute("height");
            set { SetIntAttribute("height", value); }
        }

        public string List
        {
            get => GetAttributeLower("list");
            set { SetAttributeLower("list", value); }
        }

        public string Max
        {
            get => GetAttributeLower("max");
            set { SetAttributeLower("max", value); }
        }

        public int? MaxLength
        {
            get => GetIntAttribute("maxlength");
            set { SetIntAttribute("maxlength", value); }
        }

        public string Min
        {
            get => GetAttributeLower("min");
            set { SetAttributeLower("min", value); }
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

        public string Pattern
        {
            get => GetAttributeLower("pattern");
            set { SetAttributeLower("pattern", value); }
        }

        public string Placeholder
        {
            get => GetAttributeLower("placeholder");
            set { SetAttributeLower("placeholder", value); }
        }

        public bool Readonly
        {
            get => HasAttributeLower("readonly");
            set { SetFlagAttributeLower("readonly", value); }
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

        public string Src
        {
            get => GetAttributeLower("src");
            set { SetAttributeLower("src", value); }
        }

        public string Step
        {
            get => GetAttributeLower("step");
            set { SetAttributeLower("step", value); }
        }

        public string Type
        {
            get => GetAttributeLower("type");
            set { SetAttributeLower("type", value); }
        }

        public string Value
        {
            get => GetAttributeLower("value");
            set { SetAttributeLower("value", value); }
        }

        public int? Width
        {
            get => GetIntAttribute("width");
            set { SetIntAttribute("width", value); }
        }
    }
}
