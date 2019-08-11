/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using Integrative.Lara;

namespace SampleProject
{
    [LaraWebComponent(MyCheckbox)]
    class CheckboxComponent : WebComponent
    {
        public const string MyCheckbox = "my-checkbox";

        readonly Input _checkbox;
        readonly Element _label;

        public CheckboxComponent() : base(MyCheckbox)
        {
            _checkbox = new Input();
            _label = Create("label");
            AttachShadow();
            var builder = new LaraBuilder(ShadowRoot);
            builder.Push("div", "form-group form-check")
                .Push(_checkbox, "form-check-input")
                    .Attribute("type", "checkbox")
                .Pop()
                .Push(_label)
                    .Attribute("for", _checkbox.EnsureElementId())
                .Pop()
            .Pop();
        }

        public bool Checked
        {
            get => _checkbox.Checked;
            set => _checkbox.Checked = value;
        }

        string _labelText;

        public string Label
        {
            get => _labelText;
            set
            {
                _labelText = value;
                _label.SetInnerText(value);
            }
        }

        protected override IEnumerable<string> GetObservedAttributes()
        {
            return new string[] { "checked", "label", "class" };
        }

        protected override void OnAttributeChanged(string attribute)
        {
            if (attribute == "checked")
            {
                Checked = HasAttribute("checked");
            }
            else if (attribute == "label")
            {
                Label = GetAttribute("label");
            }
            else if (attribute == "class")
            {
                var root = _checkbox.ParentElement;
                root.Class = GetAttribute("class");
                root.AddClass("form-group");
                root.AddClass("form-check");
            }
        }
    }
}
