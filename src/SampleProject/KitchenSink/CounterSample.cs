/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Globalization;
using System.Threading.Tasks;
using Integrative.Lara;

namespace SampleProject.KitchenSink
{
    internal sealed class CounterSample
    {
        private readonly InputElement _number;
        
        public Element Root { get; }

        public CounterSample()
        {
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
                .Push("button", "btn btn-primary")
                    .InnerText("Increase")
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
