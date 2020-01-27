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
    internal class SelectSample
    {
        public Element Root { get; }

        public SelectSample()
        {
            var @select = new SelectElement
            {
                Id = "myselect",
                Class = "form-control",
            };
            var advance = new Button
            {
                Class = "btn btn-primary"
            };
            advance.AppendChild(new TextNode("Advance"));
            @select.AddOption("0", "Monday");
            @select.AddOption("1", "Tuesday");
            @select.AddOption("2", "Wednesday");
            @select.AddOption("3", "Thursday");
            @select.AddOption("4", "Friday");
            @select.AddOption("5", "Saturday");
            @select.AddOption("6", "Sunday");
            advance.On("click", () =>
            {
                int.TryParse(@select.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int weekday);
                weekday = (weekday + 1) % 7;
                @select.Value = weekday.ToString(CultureInfo.InvariantCulture);
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
                .Push(advance)
                .Pop()
            .Pop();
        }

    }
}
