/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class Script : Element
    {
        public Script() : base("script")
        {
        }

        public bool Async
        {
            get => HasAttributeLower("async");
            set { SetFlagAttributeLower("async", value); }
        }

        public string Charset
        {
            get => GetAttributeLower("charset");
            set { SetAttributeLower("charset", value); }
        }

        public bool Defer
        {
            get => HasAttributeLower("defer");
            set { SetFlagAttributeLower("defer", value); }
        }

        public string Src
        {
            get => GetAttributeLower("src");
            set { SetAttributeLower("src", value); }
        }

        public string Type
        {
            get => GetAttributeLower("type");
            set { SetAttributeLower("type", value); }
        }
    }
}
