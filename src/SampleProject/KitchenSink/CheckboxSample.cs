/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using Integrative.Lara;

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
            ShadowRoot.Children = new List<Node>
            {
                new HtmlDivElement
                {
                    Class = "form-row",
                    Children = new List<Node>
                    {
                        new HtmlDivElement
                        {
                            Class = "form-group col-md-2 my-1",
                            Children = new List<Node>
                            {
                                new HtmlDivElement
                                {
                                    Class = "form-check",
                                    Children = new List<Node>
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
                            Children = new List<Node>
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
