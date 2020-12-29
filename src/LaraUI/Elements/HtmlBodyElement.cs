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
    /// Body element
    /// </summary>
    [Obsolete("Use HtmlBodyElement")]
    [EditorBrowsable(EditorBrowsableState.Never)]
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
