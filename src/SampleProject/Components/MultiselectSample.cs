/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using Integrative.Lara;

namespace SampleProject.Components
{
    internal class MultiselectSample : WebComponent
    {
        public MultiselectSample()
        {
            var combo = new HtmlSelectElement
            {
                Class = "form-control",
                Multiple = true
            };
            combo.AddOption("N", "North");
            combo.AddOption("E", "East");
            combo.AddOption("S", "South");
            combo.AddOption("W", "West");
            var toggle = new HtmlButtonElement
            {
                Class = "btn btn-primary",
                InnerText = "Toggle"
            };
            toggle.On("click", () =>
            {
                foreach (var child in combo.Children)
                {
                    if (child is not HtmlOptionElement option) continue;
                    option.Selected = !option.Selected;
                }
            });
            ShadowRoot.Children = new Node[]
            {
                new HtmlDivElement
                {
                    Class = "form-row",
                    Children = new Node[]
                    {
                        new HtmlDivElement
                        {
                            Class = "form-group col-md-2",
                            Children = new Node[] { combo }
                        },
                        new HtmlDivElement
                        {
                            Class = "form-group col-md-1",
                            Children = new Node[] { toggle }
                        }
                    }
                }
            };
        }
    }
}
