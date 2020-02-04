/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Integrative.Lara
{
    internal static class HtmlReference
    {
        private static readonly HashSet<string> _SelfClosingTags;
        private static readonly HashSet<string> _RequiresId;

        [SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline", Justification = "Required behavior")]
        static HtmlReference()
        {
            _SelfClosingTags = new HashSet<string>
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
            _RequiresId = new HashSet<string>
            {
                "input", "textarea", "select", "button", "option"
            };
        }

        public static bool IsSelfClosingTag(string tagNameLower)
            => _SelfClosingTags.Contains(tagNameLower);

        public static bool RequiresId(string tagNameLower)
            => _RequiresId.Contains(tagNameLower);
    }
}
