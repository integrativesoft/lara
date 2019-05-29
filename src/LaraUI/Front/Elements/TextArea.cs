/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class TextArea : Element
    {
        public TextArea() : base("textarea")
        {
        }

        public bool Autofocus
        {
            get => HasAttributeLower("autofocus");
            set { SetFlagAttributeLower("autofocus", value); }
        }

        public int? Cols
        {
            get => GetIntAttribute("cols");
            set { SetIntAttribute("cols", value); }
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

        public int? MaxLength
        {
            get => GetIntAttribute("maxlength");
            set { SetIntAttribute("maxlength", value); }
        }

        public string Name
        {
            get => GetAttributeLower("name");
            set { SetAttributeLower("name", value); }
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

        public int? Rows
        {
            get => GetIntAttribute("rows");
            set { SetIntAttribute("rows", value); }
        }

        public string Value
        {
            get => GetAttributeLower("value");
            set { SetAttributeLower("value", value); }
        }

        public string Wrap
        {
            get => GetAttributeLower("wrap");
            set { SetAttributeLower("wrap", value); }
        }
    }
}
