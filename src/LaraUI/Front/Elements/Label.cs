/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    /// <summary>
    /// The 'label' HTML5 element.
    /// </summary>
    /// <seealso cref="Integrative.Lara.Element" />
    public sealed class Label : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Label"/> class.
        /// </summary>
        public Label() : base("label")
        {
        }

        /// <summary>
        /// Gets or sets the 'for' HTML5 attribute.
        /// </summary>
        public string For
        {
            get => GetAttributeLower("for");
            set { SetAttributeLower("for", value); }
        }
    }
}
