/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;
using Integrative.Lara;

namespace SampleProject.KitchenSink
{
    internal class MultiselectSample
    {
        public Element Root { get; }

        public MultiselectSample()
        {
            var @select = new HtmlSelectElement
            {
                Id = "mymulti",
                Class = "form-control",
                Multiple = true
            };
            var toggle = new HtmlButtonElement
            {
                Class = "btn btn-primary"
            };
            toggle.AppendChild(new TextNode("Toggle"));
            @select.AddOption("N", "North");
            @select.AddOption("E", "East");
            @select.AddOption("S", "South");
            @select.AddOption("W", "West");
            toggle.On("click", () =>
            {
                foreach (var child in @select.Children)
                {
                    if (child is HtmlOptionElement option)
                    {
                        option.Selected = !option.Selected;
                    }
                }
                return Task.CompletedTask;
            });
            Root = Element.Create("div");
            Root.Class = "form-row";
            var builder = new LaraBuilder(Root);
            builder
            .Push("div", "form-group col-md-2")
                .Push(@select)
                .Pop()
            .Pop()
            .Push("div", "form-group col-md-1")
                .Push(toggle)
                .Pop()
            .Pop();
        }
    }
}
