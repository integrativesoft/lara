/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    class MultiselectSample
    {
        readonly Select _select;
        readonly Button _toggle;

        public MultiselectSample()
        {
            _select = new Select
            {
                Id = "mymulti",
                Class = "form-control",
                Multiple = true
            };
            _toggle = new Button
            {
                Class = "btn btn-primary"
            };
            _toggle.AppendChild(new TextNode("Toggle"));
            _select.AddOption("N", "North");
            _select.AddOption("E", "East");
            _select.AddOption("S", "South");
            _select.AddOption("W", "West");
            _toggle.On("click", () =>
            {
                foreach (var child in _select.Children)
                {
                    if (child is Option option)
                    {
                        option.Selected = !option.Selected;
                    }
                }
                return Task.CompletedTask;
            });
        }

        public Element Build()
        {
            var row = Element.Create("div");
            row.Class = "form-row";

            var builder = new LaraBuilder(row);
            builder
            .Push("div", "form-group col-md-2")
                .Push(_select)
                .Pop()
            .Pop()
            .Push("div", "form-group col-md-1")
                .Push(_toggle)
                .Pop()
            .Pop();

            return row;
        }
    }
}
