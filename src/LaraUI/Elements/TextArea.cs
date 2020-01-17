/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    /// <summary>
    /// The 'textarea' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public sealed class TextArea : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextArea"/> class.
        /// </summary>
        public TextArea() : base("textarea")
        {
        }

        internal override void NotifyValue(ElementEventValue entry)
        {
            base.NotifyValue(entry);
            NotifyValue(entry.Value);
        }

        /// <summary>
        /// Gets or sets the 'autofocus' HTML5 attribute.
        /// </summary>
        public bool Autofocus
        {
            get => HasAttributeLower("autofocus");
            set { SetFlagAttributeLower("autofocus", value); }
        }

        /// <summary>
        /// Gets or sets the 'cols' HTML5 attribute.
        /// </summary>
        public int? Cols
        {
            get => GetIntAttribute("cols");
            set { SetIntAttribute("cols", value); }
        }

        /// <summary>
        /// Gets or sets the 'dirname' HTML5 attribute.
        /// </summary>
        public string? Dirname
        {
            get => GetAttributeLower("dirname");
            set { SetAttributeLower("dirname", value); }
        }

        /// <summary>
        /// Gets or sets the 'disabled' HTML5 attribute.
        /// </summary>
        public bool Disabled
        {
            get => HasAttributeLower("disabled");
            set { SetFlagAttributeLower("disabled", value); }
        }

        /// <summary>
        /// Gets or sets the 'maxlength' HTML5 attribute.
        /// </summary>
        public int? MaxLength
        {
            get => GetIntAttribute("maxlength");
            set { SetIntAttribute("maxlength", value); }
        }

        /// <summary>
        /// Gets or sets the 'name' HTML5 attribute.
        /// </summary>
        public string? Name
        {
            get => GetAttributeLower("name");
            set { SetAttributeLower("name", value); }
        }

        /// <summary>
        /// Gets or sets the 'placeholder' HTML5 attribute.
        /// </summary>
        public string? Placeholder
        {
            get => GetAttributeLower("placeholder");
            set { SetAttributeLower("placeholder", value); }
        }

        /// <summary>
        /// Gets or sets the 'readonly' HTML5 attribute.
        /// </summary>
        public bool Readonly
        {
            get => HasAttributeLower("readonly");
            set { SetFlagAttributeLower("readonly", value); }
        }

        /// <summary>
        /// Gets or sets the 'readonly' HTML5 attribute.
        /// </summary>
        public bool Required
        {
            get => HasAttributeLower("required");
            set { SetFlagAttributeLower("required", value); }
        }

        /// <summary>
        /// Gets or sets the 'rows' HTML5 attribute.
        /// </summary>
        public int? Rows
        {
            get => GetIntAttribute("rows");
            set { SetIntAttribute("rows", value); }
        }

        /// <summary>
        /// Gets or sets the 'value' property.
        /// </summary>
        public string? Value
        {
            get => GetAttributeLower("value");
            set { SetAttributeLower("value", value); }
        }

        /// <summary>
        /// Gets or sets the 'wrap' HTML5 attribute.
        /// </summary>
        public string? Wrap
        {
            get => GetAttributeLower("wrap");
            set { SetAttributeLower("wrap", value); }
        }
    }
}
