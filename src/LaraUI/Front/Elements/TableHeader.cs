/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class TableHeader : Element
    {
        public TableHeader() : base("th")
        {
        }

        public string Abbr
        {
            get => GetAttributeLower("abbr");
            set { SetAttributeLower("abbr", value); }
        }

        public int? ColSpan
        {
            get => GetIntAttribute("colspan");
            set { SetIntAttribute("colspan", value); }
        }

        public string Headers
        {
            get => GetAttributeLower("headers");
            set { SetAttributeLower("headers", value); }
        }

        public int? RowSpan
        {
            get => GetIntAttribute("rowspan");
            set { SetIntAttribute("rowspan", value); }
        }

        public string Scope
        {
            get => GetAttributeLower("scope");
            set { SetAttributeLower("scope", value); }
        }

        public string Sorted
        {
            get => GetAttributeLower("sorted");
            set { SetAttributeLower("sorted", value); }
        }
    }
}
