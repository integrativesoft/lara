/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 12/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Head element
    /// </summary>
    [Obsolete("Use HtmlHeadElement instead")]
    public class HeadElement : HtmlHeadElement
    {
    }

    /// <summary>
    /// HTML 'head' element
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlHeadElement : Element
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HtmlHeadElement() : base("head")
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items"></param>
        public HtmlHeadElement(params Node[] items) : base("head", items)
        {
        }

    }
}
