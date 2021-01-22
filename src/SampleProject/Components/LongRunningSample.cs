/*
Copyright (c) 2020-2021 Integrative Software LLC
Created: 1/2020
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject.Components
{
    internal class LongRunningSample : WebComponent
    {
        private static readonly string[] _Steps = {
            "Putting grounds into container",
            "Covering coffee and water mixture",
            "Filtering coffee and water mixture",
            "Serving..."
        };

        public LongRunningSample()
        {
            ShadowRoot.Children = new Node[]
            {
                new HtmlDivElement
                {
                    Class = "form-row",
                    Children = new Node[]
                    {
                        new HtmlButtonElement
                        {
                            Class = "btn btn-primary my-2",
                            InnerText = "Long-running action"
                        }
                        .Extract(out var button)
                    }
                },
                new HtmlDivElement
                {
                    Class = "card text-center",
                    Style = "display: none; width: 18rem",
                    Children = new Node[]
                    {
                        new HtmlImageElement
                        {
                            Class = "card-img-top mt-2",
                            Height = "100",
                            Src = "/Coffee.svg",
                            Alt = "Coffee mug"
                        },
                        new HtmlDivElement
                        {
                            Class = "card-body",
                            Children = new Node[]
                            {
                                new HtmlHeadingElement(5)
                                {
                                    Class = "card-title",
                                    InnerText = "Preparing..."
                                },
                                new HtmlParagraphElement
                                {
                                    Class = "card=-text"
                                }
                                .Extract(out var text)
                            }
                        }
                    }
                }
                .Extract(out var card)
            };
            button.On(new EventSettings
            {
                EventName = "click",
                LongRunning = true,
                BlockOptions = new BlockOptions
                {
                    ShowElementId = card.Id
                },
                Handler = async () =>
                {
                    foreach (var step in _Steps)
                    {
                        text.InnerText = step;
                        await LaraUI.FlushPartialChanges();
                        await Task.Delay(800);
                    }
                    text.ClearChildren();
                }
            });
        }
    }
}
