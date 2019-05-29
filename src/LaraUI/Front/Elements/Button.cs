/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class Button : Element
    {
        public Button() : base("button")
        {
            Type = "button";
        }

        public bool AutoFocus
        {
            get => HasAttributeLower("autofocus");
            set { SetFlagAttributeLower("autofocus", value); }
        }

        public bool Disabled
        {
            get => HasAttribute("disabled");
            set { SetFlagAttributeLower("disabled", value); }
        }

        public string Name
        {
            get => GetAttributeLower("name");
            set { SetAttributeLower("name", value); }
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
    }
}
