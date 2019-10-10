/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Globalization;
using System.Threading.Tasks;

namespace SampleProject
{
    class SelectSample
    {
        readonly SelectElement _select;
        readonly Button _advance;

        public Element Root { get; }

        public SelectSample()
        {
            _select = new SelectElement
            {
                Id = "myselect",
                Class = "form-control",
            };
            _advance = new Button
            {
                Class = "btn btn-primary"
            };
            _advance.AppendChild(new TextNode("Advance"));
            _select.AddOption("0", "Monday");
            _select.AddOption("1", "Tuesday");
            _select.AddOption("2", "Wednesday");
            _select.AddOption("3", "Thursday");
            _select.AddOption("4", "Friday");
            _select.AddOption("5", "Saturday");
            _select.AddOption("6", "Sunday");
            _advance.On("click", () =>
            {
                int.TryParse(_select.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out int weekday);
                weekday = (weekday + 1) % 7;
                _select.Value = weekday.ToString(CultureInfo.InvariantCulture);
                return Task.CompletedTask;
            });
            Root = Element.Create("div");
            Root.Class = "form-row";
            var builder = new LaraBuilder(Root);
            builder
            .Push("div", "form-group col-md-2")
                .Push(_select)
                .Pop()
            .Pop()
            .Push("div", "form-group col-md-1")
                .Push(_advance)
                .Pop()
            .Pop();
        }

    }
}
