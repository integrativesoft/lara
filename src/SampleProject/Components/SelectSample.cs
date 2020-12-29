/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using Integrative.Lara;

namespace SampleProject.Components
{
    internal class SelectSample : WebComponent
    {
        public int Test { get; init; }

        public SelectSample()
        {
            ShadowRoot.Child(
                new HtmlDivElement { Class = "form-row" }.Child(
                    new HtmlDivElement { Class = "form-group col-md-2" }.Child(
                        new WeekdayCombo().Extract(out var combo)
                    ),
                    new HtmlDivElement { Class = "form-group col-md-1"}.Child(
                        new HtmlButtonElement
                            { InnerText = "Advance", Class ="btn btn-primary" }
                            .Event("click", () => combo.NextDay())
                    )
                ));
        }
    }
}
