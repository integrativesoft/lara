/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    class CheckboxSample
    {
        readonly Input _checkbox;
        readonly Button _toggle;

        public Element Root { get; }

        public CheckboxSample()
        {
            _checkbox = new Input
            {
                Id = "mycheckbox",
                Type = "checkbox",
                Class = "form-check-input"
            };
            _toggle = new Button
            {
                Class = "btn btn-primary",
            };
            _toggle.On("click", () =>
            {
                _checkbox.Checked = !_checkbox.Checked;
                return Task.CompletedTask;
            });
            Root = Element.Create("div");
            Root.Class = "form-row";
            var builder = new LaraBuilder(Root);
            builder.Push("div", "form-group col-md-2 my-1")
                .Push("div", "form-check")
                    .Push(_checkbox)
                    .Pop()
                    .Push("label", "form-check-label")
                        .Attribute("for", _checkbox.Id)
                        .AppendText("Check me out")
                    .Pop()
                .Pop()
            .Pop()
            .Push("div", "form-group col-md-1")
                .Push(_toggle)
                    .AppendText("Toggle")
                .Pop()
            .Pop();
        }
    }
}
