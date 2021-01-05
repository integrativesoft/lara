/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using Integrative.Lara;
using SampleProject.Common;

namespace SampleProject.Components
{
    internal class KitchenSinkComponent : WebComponent
    {
        public KitchenSinkComponent()
        {
            ShadowRoot.Children = new Node[]
            {
                new HtmlDivElement
                {
                    Class = "container p-4",
                    Children = new Node[]
                    {
                        new CounterSample(),
                        new CheckboxSample(),
                        new MultiselectSample(),
                        new LockingSample(),
                        new LongRunningSample(),
                        new CountrySelector(),
                        new HtmlDivElement
                        {
                            Class = "mt-3",
                            Children = new Node[]
                            {
                                new HtmlDivElement
                                {
                                    Class = "mt-2",
                                    Children = new Node[]
                                    {
                                        new HtmlDivElement
                                        {
                                            Children = new Node[]
                                            {
                                                new HtmlAnchorElement
                                                {
                                                    HRef = "/upload",
                                                    Target = "_blank",
                                                    InnerText = "File upload example"
                                                }
                                            }
                                        },
                                        new HtmlDivElement
                                        {
                                            Children = new Node[]
                                            {
                                                new HtmlAnchorElement
                                                {
                                                    HRef = "/server",
                                                    Target = "_blank",
                                                    InnerText = "Server-side events"
                                                }
                                            }
                                        }
                                    }
                                },
                            }
                        }
                    }
                }
            };
        }
    }
}
