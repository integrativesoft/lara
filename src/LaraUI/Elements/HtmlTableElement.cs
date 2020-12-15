/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Table element
    /// </summary>
    [Obsolete("Use HtmlTableElement instead")]
    public class Table : HtmlTableElement
    {
    }

    /// <summary>
    /// The 'table' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlTableElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlTableElement"/> class.
        /// </summary>
        public HtmlTableElement() : base("table")
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="items"></param>
        public HtmlTableElement(params Node[] items) : base("table", items)
        {
        }
    }
}
