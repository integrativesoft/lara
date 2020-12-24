/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 12/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Body element
    /// </summary>
    [Obsolete("Use HtmlBodyElement")]
    public class BodyElement : HtmlBodyElement
    {
    }

    /// <summary>
    /// HTML 'body' element
    /// </summary>
    public class HtmlBodyElement : Element
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HtmlBodyElement() : base("body")
        {
        }
    }
}
