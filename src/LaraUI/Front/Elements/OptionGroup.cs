/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;

namespace Integrative.Lara
{
    public sealed class OptionGroup : Element
    {
        public OptionGroup() : base("optgroup")
        {
        }

        public bool Disabled
        {
            get => HasAttributeLower("disabled");
            set { SetFlagAttributeLower("disabled", value); }
        }

        public string Label
        {
            get => GetAttributeLower("label");
            set { SetAttributeLower("label", value); }
        }

        public IEnumerable<Option> Options => GetOptions();

        private IEnumerable<Option> GetOptions()
        {
            foreach (var node in Children)
            {
                if (node is Option option)
                {
                    yield return option;
                }
            }
        }

        protected override void OnChildAdded(Node child)
        {
            if (ParentElement is Select parent && child is Option option)
            {
                var value = parent.Value;
                if (!string.IsNullOrEmpty(value))
                {
                    option.NotifyAdded(value);
                }
            }
        }

        internal void NotifyAdded(string parentValue)
        {
            foreach (var child in GetOptions())
            {
                if (child.Value == parentValue)
                {
                    child.Selected = true;
                }
            }
        }
    }
}
