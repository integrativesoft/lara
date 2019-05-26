/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class TableCell : Element
    {
        public TableCell() : base("td")
        {
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
    }
}
