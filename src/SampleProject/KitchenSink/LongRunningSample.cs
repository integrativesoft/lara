/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System;
using System.Threading.Tasks;
using Integrative.Lara;

namespace SampleProject.KitchenSink
{
    internal class LongRunningSample
    {
        private readonly HtmlButtonElement _button = new HtmlButtonElement();
        private readonly Element _card = Element.Create("div");
        private readonly Element _coffeeText = Element.Create("p");
        private readonly Element _counterText = Element.Create("p");

        private readonly string[] _coffees = { "capuccino", "americano", "latte", "mocha" };
        private readonly string[] _sizes = { "small", "medium", "large" };

        private readonly string[] _steps = { "Putting grounds into container",
            "Covering coffee and water mixture",
            "Filtering coffee and water mixture",
            "Serving..."
        };

        public Element Root { get; } = Element.Create("div");

        public LongRunningSample()
        {
            Root.Class = "form-row";
            var builder = new LaraBuilder(Root);
            builder.Push(_button, "btn btn-primary my-2")
                .AppendText("Long-running action")
            .Pop()
            .Push(_card, "card text-center")
                .Attribute("style", "display: none; width: 18rem")
                .Push("img", "card-img-top mt-2")
                    .Attribute("height", "100")
                    .Attribute("src", "/Coffee.svg")
                    .Attribute("alt", "coffee mug")
                .Pop()
                .Push("div", "card-body")
                    .Push("h5", "card-title")
                        .AppendText("Preparing...")
                    .Pop()
                    .Push(_coffeeText, "card-text")
                    .Pop()
                    .Push(_counterText, "card-text")
                    .Pop()
                .Pop()
            .Pop();
            _button.On(new EventSettings
            {
                EventName = "click",
                LongRunning = true,
                Handler = ButtonHandler,
                BlockOptions = new BlockOptions
                {
                    ShowElementId = _card.EnsureElementId(),
                }
            });
        }

        private async Task ButtonHandler()
        {
            var header = BuildMessage();
            _coffeeText.InnerText = header;
            foreach (var step in _steps)
            {
                _counterText.InnerText = step;
                await LaraUI.Page.Navigation.FlushPartialChanges();
                await Task.Delay(1000);
            }
            _coffeeText.ClearChildren();
            _counterText.ClearChildren();
        }

        private string BuildMessage()
        {
            var coffee = ChooseRandom(_coffees);
            var size = ChooseRandom(_sizes);
            return $"Brewing a {size} {coffee}";
        }

        private static string ChooseRandom(string[] series)
        {
            var random = new Random();
            var min = series.GetLowerBound(0);
            var max = series.GetUpperBound(0);
            var index = random.Next(min, max + 1);
            return series[index];
        }
    }
}
