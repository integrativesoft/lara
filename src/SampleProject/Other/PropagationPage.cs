/*
Copyright (c) 2019 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject.Other
{
    [LaraPage("/propagation")]
    class PropagationPage : IPage
    {
        readonly Element _divCounter = Element.Create("div");
        int _counter;

        public Task OnGet()
        {
            var builder = new LaraBuilder(LaraUI.Page.Document.Body);
            builder.Push("div")
                .InnerText("Clicking on 'parent' or 'AllowPropagation' increases counter.")
            .Pop()
            .Push(_divCounter)
                .InnerText("0")
            .Pop()
            .Push("div")
                .Push("span")
                    .InnerText("parent")
                .Pop()
                .On("click", ParentClick)
                .Push("div")
                    .InnerText("StopPropagation")
                    .On(new EventSettings
                    {
                        EventName = "click",
                        Handler = () => Task.CompletedTask
                    })
                .Pop()
                .Push("div")
                    .InnerText("AllowPropagation")
                    .On(new EventSettings
                    {
                        EventName = "click",
                        Propagation = PropagationType.AllowAll,
                        Handler = () => Task.CompletedTask
                    })
                .Pop()
            .Pop();
            return Task.CompletedTask;
        }

        private Task ParentClick()
        {
            _counter++;
            _divCounter.InnerText = _counter.ToString();
            return Task.CompletedTask;
        }
    }
}
