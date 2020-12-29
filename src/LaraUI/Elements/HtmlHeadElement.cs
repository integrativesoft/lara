/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 12/2019
Author: Pablo Carbonell
*/

using System;
using System.ComponentModel;

namespace Integrative.Lara
{
    /// <summary>
    /// Head element
    /// </summary>
    [Obsolete("Use HtmlHeadElement instead")]
    [EditorBrowsable(EditorBrowsableState.Never)]
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
    }
}
