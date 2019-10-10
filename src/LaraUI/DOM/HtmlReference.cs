/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;

namespace Integrative.Lara.DOM
{
    static class HtmlReference
    {
        static readonly HashSet<string> _selfClosingTags;
        static readonly HashSet<string> _requiresId;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline", Justification = "Required behavior")]
        static HtmlReference()
        {
            _selfClosingTags = new HashSet<string>
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
            _requiresId = new HashSet<string>
            {
                "input", "textarea", "select", "button", "option"
            };
        }

        public static bool IsSelfClosingTag(string tagNameLower)
            => _selfClosingTags.Contains(tagNameLower);

        public static bool RequiresId(string tagNameLower)
            => _requiresId.Contains(tagNameLower);
    }
}
