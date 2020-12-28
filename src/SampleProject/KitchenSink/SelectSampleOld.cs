/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Globalization;
using System.Threading.Tasks;
using Integrative.Lara;

namespace SampleProject.KitchenSink
{
    internal class SelectSampleOld
    {
        public Element Root { get; }

        public SelectSampleOld()
        {
            var combo = new HtmlSelectElement
            {
                Id = "myselect",
                Class = "form-control",
            };
            var advance = new HtmlButtonElement
            {
                Class = "btn btn-primary"
            };
            advance.AppendChild(new TextNode("Advance"));
            combo.AddOption("0", "Monday");
            combo.AddOption("1", "Tuesday");
            combo.AddOption("2", "Wednesday");
            combo.AddOption("3", "Thursday");
            combo.AddOption("4", "Friday");
            combo.AddOption("5", "Saturday");
            combo.AddOption("6", "Sunday");
            advance.On("click", () =>
            {
                int.TryParse(combo.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var weekday);
                weekday = (weekday + 1) % 7;
                combo.Value = weekday.ToString(CultureInfo.InvariantCulture);
                return Task.CompletedTask;
            });
            Root = Element.Create("div");
            Root.Class = "form-row";
            var builder = new LaraBuilder(Root);
            builder
            .Push("div", "form-group col-md-2")
                .Push(combo)
                .Pop()
            .Pop()
            .Push("div", "form-group col-md-1")
                .Push(advance)
                .Pop()
            .Pop();
        }
    }
}
