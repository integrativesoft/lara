/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class ListItem : Element
    {
        public ListItem() : base("li")
        {
        }

        public string Value
        {
            get => GetAttributeLower("value");
            set { SetAttributeLower("value", value); }
        }
    }
}
