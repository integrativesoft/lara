/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class ColGroup : Element
    {
        public ColGroup() : base("colgroup")
        {
        }

        public int? Span
        {
            get => GetIntAttribute("span");
            set { SetIntAttribute("span", value); }
        }
    }
}
