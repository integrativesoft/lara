/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class Label : Element
    {
        public Label() : base("label")
        {
        }

        public string For
        {
            get => GetAttributeLower("for");
            set { SetAttributeLower("for", value); }
        }
    }
}
