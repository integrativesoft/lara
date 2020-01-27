/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;
using Integrative.Lara;

namespace SampleProject.KitchenSink
{
    internal class CheckboxSample
    {
        public Element Root { get; }

        public CheckboxSample()
        {
            var checkbox = new InputElement
            {
                Id = "mycheckbox",
                Type = "checkbox",
                Class = "form-check-input"
            };
            var toggle = new Button
            {
                Class = "btn btn-primary",
            };
            toggle.On("click", () =>
            {
                checkbox.Checked = !checkbox.Checked;
                return Task.CompletedTask;
            });
            Root = Element.Create("div");
            Root.Class = "form-row";
            var builder = new LaraBuilder(Root);
            builder.Push("div", "form-group col-md-2 my-1")
                .Push("div", "form-check")
                    .Push(checkbox)
                    .Pop()
                    .Push("label", "form-check-label")
                        .Attribute("for", checkbox.Id)
                        .AppendText("Check me out")
                    .Pop()
                .Pop()
            .Pop()
            .Push("div", "form-group col-md-1")
                .Push(toggle)
                    .AppendText("Toggle")
                .Pop()
            .Pop();
        }
    }
}
