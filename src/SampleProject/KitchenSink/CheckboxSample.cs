/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject.KitchenSink
{
    internal class CheckboxSample : WebComponent
    {
        public CheckboxSample()
        {
            var checkbox = new HtmlInputElement
            {
                Id = "mycheckbox",
                Type = "checkbox",
                Class = "form-check-input"
            };
            var toggle = new HtmlButtonElement
            {
                Class = "btn btn-primary",
                InnerText = "Toggle"
            };
            toggle.On("click", () =>
            {
                checkbox.Checked = !checkbox.Checked;
                return Task.CompletedTask;
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
                            Class = "form-group col-md-2 my-1",
                            Children = new Node[]
                            {
                                new HtmlDivElement
                                {
                                    Class = "form-check",
                                    Children = new Node[]
                                    {
                                        checkbox,
                                        new HtmlLabelElement
                                        {
                                            For = checkbox.Id,
                                            InnerText = "Check me out"
                                        }
                                    }
                                }
                            }
                        },
                        new HtmlDivElement
                        {
                            Class = "form-group col-md-1",
                            Children = new Node[]
                            {
                                toggle
                            }
                        }
                    }
                }
            };
        }
    }
}
