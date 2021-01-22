/*
Copyright (c) 2020-2021 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using Integrative.Lara;
using SampleProject.Common;
using System.Threading.Tasks;

namespace SampleProject.Components
{
    internal class LockingSample : WebComponent
    {
        public LockingSample()
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
                            InnerText = "Action that blocks the UI"
                        }
                        .Event(new EventSettings
                        {
                            EventName = "click",
                            Handler = () => Task.Delay(1000),
                            BlockOptions = new BlockOptions
                            {
                                ShowHtmlMessage = Tools.GetSpinnerHtml("Brewing coffee...")
                            }
                        })
                    }
                },
            };
        }
    }
}
