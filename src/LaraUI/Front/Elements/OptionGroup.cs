/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class OptionGroup : Element
    {
        public OptionGroup() : base("optgroup")
        {
        }

        public bool Disabled
        {
            get => HasAttributeLower("disabled");
            set { SetFlagAttributeLower("disabled", value); }
        }

        public string Label
        {
            get => GetAttributeLower("label");
            set { SetAttributeLower("label", value); }
        }
    }
}
