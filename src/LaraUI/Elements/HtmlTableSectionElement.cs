/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using System.Collections.Generic;

namespace Integrative.Lara
{
    /// <summary>
    /// HTML table section types
    /// </summary>
    public enum HtmlTableSectionType
    {
        /// <summary>
        /// Table body
        /// </summary>
        Body,

        /// <summary>
        /// Table header
        /// </summary>
        Head,

        /// <summary>
        /// Table footer
        /// </summary>
        Foot
    }

    /// <summary>
    /// HTML table body element
    /// </summary>
    public class HtmlTableSectionElement : Element
    {
        /// <summary>
        /// Associates a table section type with an element tag
        /// </summary>
        public static readonly Dictionary<HtmlTableSectionType, string> SectionTags
            = new Dictionary<HtmlTableSectionType, string>
            {
                { HtmlTableSectionType.Body, "tbody" },
                { HtmlTableSectionType.Head, "thead" },
                { HtmlTableSectionType.Foot, "tfoot" }
            };

        /// <summary>
        /// constructor
        /// </summary>
        public HtmlTableSectionElement(HtmlTableSectionType type)
            : base(SectionTags[type])
        {
        }
    }
}
