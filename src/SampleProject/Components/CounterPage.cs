/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    [LaraPage(PageAddress)]
    class CounterPage : IPage
    {
        public const string PageAddress = "/contercomponentpage";

        public Task OnGet()
        {
            var document = LaraUI.Page.Document;
            SampleAppBootstrap.AppendTo(document.Head);
            var counter = Element.Create("my-counter");
            document.Body.AppendChild(counter);
            counter.Class = "m-3";
            return Task.CompletedTask;
        }
    }
}
