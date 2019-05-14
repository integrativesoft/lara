/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;

namespace Integrative.Clara.DOM
{
    static class HtmlReference
    {
        readonly static HashSet<string> _selfClosingTags;

        static HtmlReference()
        {
            _selfClosingTags = new HashSet<string>()
                {
                    "area",
                    "base",
                    "br",
                    "col",
                    "command",
                    "embed",
                    "hr",
                    "img",
                    "input",
                    "keygen",
                    "link",
                    "meta",
                    "param",
                    "source",
                    "track",
                    "wbr"
                };
        }

        public static bool IsSelfClosingTag(string tagNameUpper)
        {
            return _selfClosingTags.Contains(tagNameUpper);
        }
    }
}
