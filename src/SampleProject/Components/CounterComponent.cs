/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Collections.Generic;

namespace SampleProject
{
    [LaraWebComponent(MyCounter)]
    class CounterComponent : WebComponent
    {
        public const string MyCounter = "my-counter";

        readonly Element _div;
        readonly CounterData _data;

        public CounterComponent() : base(MyCounter)
        {
            _div = Create("div");
            _data = new CounterData();
            var builder = new LaraBuilder(ShadowRoot);
            builder.Push(_div, Class)
                .Push("span")
                    .BindInnerText(_data, x => x.Counter.ToString())
                .Pop()
                .Push("button", "btn btn-primary ml-2")
                    .On("click", () => _data.Counter++)
                    .AddTextNode("increase")
                .Pop()
            .Pop();
        }

        protected override IEnumerable<string> GetObservedAttributes()
        {
            return new string[] { "class" };
        }

        protected override void OnAttributeChanged(string attribute)
        {
            if (attribute == "class")
            {
                _div.Class = Class;
            }
        }
    }

    class CounterData : BindableBase
    {
        int _counter;

        public int Counter
        {
            get => _counter;
            set => SetProperty(ref _counter, value);
        }
    }
}
