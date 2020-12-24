/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Meter element
    /// </summary>
    [Obsolete("Use HtmlMeterElement")]
    public class Meter : HtmlMeterElement
    {
    }

    /// <summary>
    /// The 'meter' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlMeterElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlMeterElement"/> class.
        /// </summary>
        public HtmlMeterElement() : base("meter")
        {
        }

        /// <summary>
        /// Gets or sets the 'high' HTML5 attribute.
        /// </summary>
        public int? High
        {
            get => GetIntAttribute("high");
            set => SetIntAttribute("high", value);
        }

        /// <summary>
        /// Gets or sets the 'low' HTML5 attribute.
        /// </summary>
        public int? Low
        {
            get => GetIntAttribute("low");
            set => SetIntAttribute("low", value);
        }

        /// <summary>
        /// Gets or sets the 'max' HTML5 attribute.
        /// </summary>
        public int? Max
        {
            get => GetIntAttribute("max");
            set => SetIntAttribute("max", value);
        }

        /// <summary>
        /// Gets or sets the 'min' HTML5 attribute.
        /// </summary>
        public int? Min
        {
            get => GetIntAttribute("min");
            set => SetIntAttribute("min", value);
        }

        /// <summary>
        /// Gets or sets the 'optimum' HTML5 attribute.
        /// </summary>
        public int? Optimum
        {
            get => GetIntAttribute("optimum");
            set => SetIntAttribute("optimum", value);
        }

        /// <summary>
        /// Gets or sets the 'value' HTML5 attribute.
        /// </summary>
        public int? Value
        {
            get => GetIntAttribute("value");
            set => SetIntAttribute("value", value);
        }
    }
}
