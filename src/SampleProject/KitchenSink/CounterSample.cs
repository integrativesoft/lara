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
        readonly InputElement _number;
        readonly Button _increase;
        
        public Element Root { get; }

        public CounterSample()
        {
            _increase = new Button
            {
                Class = "btn btn-primary"
            };
            _increase.AppendText("Increase");
            _number = new InputElement
            {
                Id = "number",
                Type = "number",
                Class = "form-control",
                Value = "5"
            };
            Root = Element.Create("div");
            Root.Class = "form-row";
            var builder = new LaraBuilder(Root);
            builder.Push("div", "form-group col-md-2")
                .Push(_number)
                .Pop()
            .Pop()
            .Push("div", "form-group col-md-1")
                .Push(_increase)
                    .On("click", OnIncrease)
                .Pop()
            .Pop();
        }

        private Task OnIncrease()
        {
            int.TryParse(_number.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number);
            number++;
            _number.Value = number.ToString(CultureInfo.InvariantCulture);
            return Task.CompletedTask;
        }
    }
}
