/*
Copyright (c) 2020-2021 Integrative Software LLC
Created: 1/2020
Author: Pablo Carbonell
*/

using Integrative.Lara;

namespace SampleProject.Components
{
    internal class CounterSample : WebComponent
    {
        private int _counter = 5;
        private int Counter { get => _counter; set => SetProperty(ref _counter, value); }

        private static int TextToInt(string? value)
        {
            return int.TryParse(value, out var result) ? result : 0;
        }

        public CounterSample()
        {
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
                            Children = new Node[]
                            {
                                new HtmlInputElement
                                {
                                    Type = "number",
                                    Class = "form-control"
                                }
                                .Bind(this, x => x.Value = Counter.ToString())
                                .BindBack(x => Counter = TextToInt(x.Value))
                            }
                        },
                        new HtmlDivElement
                        {
                            Class = "form-group col-md-1",
                            Children = new Node[]
                            {
                                new HtmlButtonElement
                                {
                                    InnerText = "Increase",
                                    Class = "btn btn-primary"
                                }
                                .Event("click", () => Counter++)
                            }
                        }
                    }
                }
            };
        }
    }
}
