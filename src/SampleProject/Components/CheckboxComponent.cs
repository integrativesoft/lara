/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using Integrative.Lara;

namespace SampleProject.Components
{
    [LaraWebComponent(MyCheckbox)]
    internal class CheckboxComponent : WebComponent
    {
        public const string MyCheckbox = "my-checkbox";

        private readonly InputElement _checkbox;
        private readonly Element _label;

        public CheckboxComponent() : base(MyCheckbox)
        {
            _checkbox = new InputElement();
            _label = Create("label");
            var builder = new LaraBuilder(ShadowRoot);
            builder.Push("div", "form-group form-check")
                .Push(_checkbox, "form-check-input")
                    .Attribute("type", "checkbox")
                    .On("click", () => UpdateLabel())
                .Pop()
                .Push(_label)
                    .Attribute("for", _checkbox.EnsureElementId())
                .Pop()
            .Pop();
        }

        private void UpdateLabel()
        {
            var text = _checkbox.Checked ? "checked" : "unchecked";
            _label.InnerText = text;
        }

        public bool Checked
        {
            get => _checkbox.Checked;
            set => _checkbox.Checked = value;
        }

        private string? _labelText;

        public string? Label
        {
            get => _labelText;
            set
            {
                _labelText = value;
                _label.InnerText = value;
            }
        }

        protected override IEnumerable<string> GetObservedAttributes()
        {
            return new[] { "checked", "label", "class" };
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
                if (root == null) return;
                root.Class = GetAttribute("class");
                root.AddClass("form-group");
                root.AddClass("form-check");
            }
        }
    }
}
