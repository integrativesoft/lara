/*
Copyright (c) 2019 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System;
using System.Threading.Tasks;

namespace SampleProject.Other
{
    [LaraPage("/async")]
    internal class ConcurrentEventPage : IPage
    {
        private readonly Element _lines = Element.Create("div");
        private readonly Random _random = new Random();

        private int _counter;

        public Task OnGet()
        {
            var builder = new LaraBuilder(LaraUI.Page.Document.Body);
            builder.Push("div")
                .Push("button")
                    .InnerText("append line")
                    .On("click", AppendHandler)
                .Pop()
            .Pop()
            .Push(_lines)
            .Pop();
            return Task.CompletedTask;
        }

        private async Task AppendHandler()
        {
            await DelayRandom();
            _counter++;
            var line = Element.Create("div");
            line.InnerText = _counter.ToString();
            _lines.AppendChild(line);            
            await DelayRandom();
        }

        private Task DelayRandom()
        {
            var interval = _random.Next(1000);
            return Task.Delay(interval);
        }
    }
}
