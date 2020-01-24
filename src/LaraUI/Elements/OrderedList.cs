/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    /// <summary>
    /// The 'ol' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public sealed class OrderedList : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderedList"/> class.
        /// </summary>
        public OrderedList() : base("ol")
        {
        }

        /// <summary>
        /// Gets or sets the 'reversed' HTML5 attribute.
        /// </summary>
        public bool Reversed
        {
            get => HasAttributeLower("reversed");
            set => SetFlagAttributeLower("reversed", value);
        }

        /// <summary>
        /// Gets or sets the 'start' HTML5 attribute.
        /// </summary>
        public int? Start
        {
            get => GetIntAttribute("start");
            set => SetIntAttribute("start", value);
        }

        /// <summary>
        /// Gets or sets the 'type' HTML5 attribute.
        /// </summary>
        public string? Type
        {
            get => GetAttributeLower("type");
            set => SetAttributeLower("type", value);
        }
    }
}
