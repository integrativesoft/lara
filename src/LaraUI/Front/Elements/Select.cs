/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using System.Collections.Generic;

namespace Integrative.Lara
{
    public sealed class Select : Element
    {
        public Select() : base("select")
        {
        }

        internal override void NotifyValue(ElementEventValue entry)
        {
            base.NotifyValue(entry);
            NotifyValue(entry.Value);
        }

        public void AddOption(string value, string text)
        {
            var option = new Option
            {
                Value = value
            };
            option.AppendChild(new TextNode(text));
            AppendChild(option);
        }

        public bool Autofocus
        {
            get => HasAttributeLower("autofocus");
            set { SetFlagAttributeLower("autofocus", value); }
        }

        public bool Disabled
        {
            get => HasAttributeLower("disabled");
            set { SetFlagAttributeLower("disabled", value); }
        }

        public bool Multiple
        {
            get => HasAttributeLower("multiple");
            set { SetFlagAttributeLower("multiple", value); }
        }

        public string Name
        {
            get => GetAttributeLower("name");
            set { SetAttributeLower("name", value); }
        }

        public bool Required
        {
            get => HasAttributeLower("required");
            set { SetFlagAttributeLower("required", value); }
        }

        public int? Size
        {
            get => GetIntAttribute("size");
            set { SetIntAttribute("size", value); }
        }

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

        protected override void OnChildAdded(Node child)
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
