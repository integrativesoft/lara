/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject.Other
{
    [LaraPage("/evalfilter")]
    internal class EvalFilterPage : IPage
    {
        private readonly Element _span = Element.Create("span");
        private int _counter;

        public Task OnGet()
        {
            var builder = new LaraBuilder(LaraUI.Page.Document.Body);
            builder.Push("div")
                .Push("input")
                .Pop()
            .Pop()
            .Push("div")
                .Push(_span)
                .Pop()
            .Pop();
            LaraUI.Page.Document.On(new EventSettings
            {
                EventName = "keyup",
                EvalFilter = "event.keyCode === 13",
                Handler = () =>
                {
                    _counter++;
                    _span.InnerText = _counter.ToString();
                    return Task.CompletedTask;
                }
            });
            return Task.CompletedTask;
        }        
    }
}
