/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using Integrative.Lara;

namespace SampleProject.Components
{
    [LaraWebComponent(MyCounter)]
    internal class CounterComponent : WebComponent
    {
        private const string MyCounter = "my-counter";

        private readonly Element _div;

        public CounterComponent() : base(MyCounter)
        {
            _div = Create("div");
            var data = new CounterData();
            var builder = new LaraBuilder(ShadowRoot);
            builder.Push(_div, Class)
                .Push("span")
                    .BindInnerText(data, x => x.Counter.ToString())
                .Pop()
                .Push("button", "btn btn-primary ml-2")
                    .On("click", () => data.Counter++)
                    .AppendText("increase")
                .Pop()
            .Pop();
        }

        protected override IEnumerable<string> GetObservedAttributes()
        {
            return new[] { "class" };
        }

        protected override void OnAttributeChanged(string attribute)
        {
            if (attribute == "class")
            {
                _div.Class = Class;
            }
        }
    }

    internal class CounterData : BindableBase
    {
        private int _counter;

        public int Counter
        {
            get => _counter;
            set => SetProperty(ref _counter, value);
        }
    }
}
