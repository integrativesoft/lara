/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    /// <summary>
    /// The 'colgroup' HTML5 element
    /// </summary>
    /// <seealso cref="Element" />
    public sealed class ColGroup : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColGroup"/> class.
        /// </summary>
        public ColGroup() : base("colgroup")
        {
        }

        /// <summary>
        /// Gets or sets the 'span' HTML5 attribute.
        /// </summary>
        public int? Span
        {
            get => GetIntAttribute("span");
            set => SetIntAttribute("span", value);
        }
    }
}
