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
    sealed class CounterSample
    {
        readonly Input _number;
        readonly Button _increase;

        public CounterSample()
        {
            _increase = new Button();
            _increase.AppendChild(new TextNode("Increase"));
            _increase.Class = "btn btn-primary";
            _number = new Input
            {
                Id = "number",
                Type = "number",
                Class = "form-control",
                Value = "5"
            };
            _increase.On("click", OnIncrease);
        }

        private Task OnIncrease()
        {
            int.TryParse(_number.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number);
            number++;
            _number.Value = number.ToString(CultureInfo.InvariantCulture);
            return Task.CompletedTask;
        }

        public Element Build()
        {
            var row = Element.Create("div");
            row.Class = "form-row";

            var builder = new LaraBuilder(row);
            builder
            .Push("div", "form-group col-md-2")
                .Push(_number)
                .Pop()
            .Pop()
            .Push("div", "form-group col-md-1")
                .Push(_increase)
                .Pop()
            .Pop();

            return row;
        }
    }
}
