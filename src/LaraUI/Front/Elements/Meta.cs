/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class Meta : Element
    {
        public Meta() : base("meta")
        {
        }

        public string Content
        {
            get => GetAttributeLower("content");
            set { SetAttributeLower("content", value); }
        }

        public string HttpEquiv
        {
            get => GetAttributeLower("http-equiv");
            set { SetAttributeLower("http-equiv", value); }
        }

        public string Name
        {
            get => GetAttributeLower("name");
            set { SetAttributeLower("name", value); }
        }
    }
}
