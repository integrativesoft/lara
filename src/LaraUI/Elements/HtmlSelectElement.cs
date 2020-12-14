/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;

namespace Integrative.Lara
{
    /// <summary>
    /// Select element
    /// </summary>
    [Obsolete("Use HtmlSelectElement instead")]
    public class SelectElement : HtmlSelectElement
    {
    }

    /// <summary>
    /// The 'select' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlSelectElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlSelectElement"/> class.
        /// </summary>
        public HtmlSelectElement() : base("select")
        {
        }

        internal override void NotifyValue(ElementEventValue entry)
        {
            base.NotifyValue(entry);
            NotifyValue(entry.Value);
        }

        /// <summary>
        /// Adds an option.
        /// </summary>
        /// <param name="value">The option's value.</param>
        /// <param name="text">The option's text.</param>
        public void AddOption(string value, string text)
        {
            var option = new HtmlOptionElement
            {
                Value = value
            };
            option.AppendChild(new TextNode(text));
            AppendChild(option);
        }

        /// <summary>
        /// Gets or sets the 'autofocus' HTML5 attribute.
        /// </summary>
        public bool Autofocus
        {
            get => HasAttributeLower("autofocus");
            set => SetFlagAttributeLower("autofocus", value);
        }

        /// <summary>
        /// Gets or sets the 'disabled' HTML5 attribute.
        /// </summary>
        public bool Disabled
        {
            get => HasAttributeLower("disabled");
            set => SetFlagAttributeLower("disabled", value);
        }

        /// <summary>
        /// Gets or sets the 'multiple' HTML5 attribute.
        /// </summary>
        public bool Multiple
        {
            get => HasAttributeLower("multiple");
            set => SetFlagAttributeLower("multiple", value);
        }

        /// <summary>
        /// Gets or sets the 'name' HTML5 attribute.
        /// </summary>
        public string? Name
        {
            get => GetAttributeLower("name");
            set => SetAttributeLower("name", value);
        }

        /// <summary>
        /// Gets or sets the 'required' HTML5 attribute.
        /// </summary>
        public bool Required
        {
            get => HasAttributeLower("required");
            set => SetFlagAttributeLower("required", value);
        }

        /// <summary>
        /// Gets or sets the 'size' HTML5 attribute.
        /// </summary>
        public int? Size
        {
            get => GetIntAttribute("size");
            set => SetIntAttribute("size", value);
        }

        /// <summary>
        /// Gets or sets the 'value' HTML5 attribute.
        /// </summary>
        public string? Value
        {
            get => GetAttributeLower("value");
            set => SetAttributeLower("value", value);
        }

        internal override void AttributeChanged(string attribute, string? value)
        {
            if (attribute == "value")
            {
                UpdateChildOptions(value);
            }
        }

        private void UpdateChildOptions(string? value)
        {
            if (Multiple)
            {
                SelectNonExclusiveOption(value);
            }
            else
            {
                SelectOnlyOption(value);
            }
        }

        private void SelectNonExclusiveOption(string? value)
        {
            foreach (var option in GetOptions())
            {
                if (option.Value == value)
                {
                    option.Selected = true;
                }
            }
        }

        private void SelectOnlyOption(string? value)
        {
            foreach (var option in GetOptions())
            {
                option.Selected = (option.Value == value);
            }
        }

        private protected override void OnChildAdded(Node child)
        {
            var value = Value;
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            if (child is HtmlOptionElement option)
            {
                option.NotifyAdded(value);
            }
            else if (child is HtmlOptionGroupElement group)
            {
                group.NotifyAdded(value);
            }
        }

        /// <summary>
        /// Gets the child options.
        /// </summary>
        public IEnumerable<HtmlOptionElement> Options => GetOptions();

        private IEnumerable<HtmlOptionElement> GetOptions()
        {
            foreach (var child in Children)
            {
                if (child is HtmlOptionElement option)
                {
                    yield return option;
                }
                else if (child is HtmlOptionGroupElement group)
                {
                    foreach (var grandchild in group.Options)
                    {
                        yield return grandchild;
                    }
                }
            }
        }
    }
}
