/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    [LaraPage(Address = PageAddress)]
    class CheckboxPage : IPage
    {
        public const string PageAddress = "/checkbox";

        public Task OnGet()
        {
            var document = LaraUI.Page.Document;
            SampleAppBootstrap.AppendTo(document.Head);
            var builder = new LaraBuilder(document.Body);
            builder.Push("my-checkbox", "m-3")
                .Attribute("label", "check me out")
            .Pop();
            return Task.CompletedTask;
        }
    }
}
