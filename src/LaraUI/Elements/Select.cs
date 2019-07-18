/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using System.Collections.Generic;

namespace Integrative.Lara
{
    /// <summary>
    /// The 'select' HTML5 element.
    /// </summary>
    /// <seealso cref="Element" />
    public sealed class Select : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Select"/> class.
        /// </summary>
        public Select() : base("select")
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
            var option = new Option
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
            set { SetFlagAttributeLower("autofocus", value); }
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
        /// Gets or sets the 'multiple' HTML5 attribute.
        /// </summary>
        public bool Multiple
        {
            get => HasAttributeLower("multiple");
            set { SetFlagAttributeLower("multiple", value); }
        }

        /// <summary>
        /// Gets or sets the 'name' HTML5 attribute.
        /// </summary>
        public string Name
        {
            get => GetAttributeLower("name");
            set { SetAttributeLower("name", value); }
        }

        /// <summary>
        /// Gets or sets the 'required' HTML5 attribute.
        /// </summary>
        public bool Required
        {
            get => HasAttributeLower("required");
            set { SetFlagAttributeLower("required", value); }
        }

        /// <summary>
        /// Gets or sets the 'size' HTML5 attribute.
        /// </summary>
        public int? Size
        {
            get => GetIntAttribute("size");
            set { SetIntAttribute("size", value); }
        }

        /// <summary>
        /// Gets or sets the 'value' HTML5 attribute.
        /// </summary>
        public string Value
        {
            get => GetAttributeLower("value");
            set
            {
                SetAttributeLower("value", value);
                UpdateChildOptions(value);
            }
        }

        private void UpdateChildOptions(string value)
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

        private void SelectNonExclusiveOption(string value)
        {
            foreach (var option in GetOptions())
            {
                if (option.Value == value)
                {
                    option.Selected = true;
                }
            }
        }

        private void SelectOnlyOption(string value)
        {
            foreach (var option in GetOptions())
            {
                option.Selected = (option.Value == value);
            }
        }

        internal override void OnChildAdded(Node child)
        {
            string value = Value;
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            if (child is Option option)
            {
                option.NotifyAdded(value);
            }
            else if (child is OptionGroup group)
            {
                group.NotifyAdded(value);
            }
        }

        /// <summary>
        /// Gets the child options.
        /// </summary>
        public IEnumerable<Option> Options => GetOptions();

        private IEnumerable<Option> GetOptions()
        {
            foreach (var child in Children)
            {
                if (child is Option option)
                {
                    yield return option;
                }
                else if (child is OptionGroup group)
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
