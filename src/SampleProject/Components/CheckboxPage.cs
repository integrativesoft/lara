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
    [LaraPage(Address = PageAddress)]
    internal class CheckboxPage : IPage
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
