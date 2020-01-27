/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;
using Integrative.Lara;
using SampleProject.Main;

namespace SampleProject.Components
{
    [LaraPage(PageAddress)]
    internal class CardPage : IPage
    {
        public const string PageAddress = "/card";

        public Task OnGet()
        {
            var document = LaraUI.Page.Document;
            SampleAppBootstrap.AppendTo(document.Head);
            var builder = new LaraBuilder(document.Body);
            builder.Push("my-card", "m-3")
                .Attribute("heading", "this is the title")
                .Attribute("subtitle", "this is the subtitle")
                .Push("span")
                    .AppendText("text 1")
                .Pop()
                .Push("br")
                .Pop()
                .Push("span")
                    .AppendText("text 2")
                .Pop()
            .Pop();
            return Task.CompletedTask;
        }
    }
}
