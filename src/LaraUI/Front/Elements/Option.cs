/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;

namespace Integrative.Lara
{
    public sealed class Option : Element
    {
        public Option() : base("option")
        {
        }

        internal override void NotifyValue(ElementEventValue entry)
        {
            base.NotifyValue(entry);
            NotifySelected(entry.Checked);
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

        public bool Selected
        {
            get => HasAttributeLower("selected");
            set { SetFlagAttributeLower("selected", value); }
        }

        public string Value
        {
            get => GetAttributeLower("value");
            set { SetAttributeLower("value", value); }
        }
    }
}
