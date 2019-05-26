/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class OrderedList : Element
    {
        public OrderedList() : base("ol")
        {
        }

        public bool Reversed
        {
            get => HasAttributeLower("reversed");
            set { SetFlagAttributeLower("reversed", value); }
        }

        public int? Start
        {
            get => GetIntAttribute("start");
            set { SetIntAttribute("start", value); }
        }

        public string Type
        {
            get => GetAttributeLower("type");
            set { SetAttributeLower("type", value); }
        }
    }
}
