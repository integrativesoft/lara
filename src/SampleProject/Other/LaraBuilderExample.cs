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
    class LaraBuilderExample : IPage
    {
        public const string PageAddress = "/builderexample";

        public Task OnGet()
        {
            var document = LaraUI.Page.Document;
            SampleAppBootstrap.AppendTo(document.Head);
            var builder = new LaraBuilder(document.Body);

            builder.Push("div", "container")
                .Push("table", "table table-hover")
                    .Push("thead", "thead-light")
                        .Push("tr")
                            .Push("th")
                                .Attribute("scope", "col")
                                .AddTextNode("First name")
                            .Pop()
                            .Push("th")
                                .Attribute("scope", "col")
                                .AddTextNode("Last name")
                            .Pop()
                        .Pop()
                        .Push("tbody")
                            .Push("tr")
                                .Push("td").AddTextNode("John").Pop()
                                .Push("td").AddTextNode("Jones").Pop()
                            .Pop()
                            .Push("tr")
                                .Push("td").AddTextNode("Amy").Pop()
                                .Push("td").AddTextNode("Smith").Pop()
                            .Pop()
                        .Pop()
                    .Pop()
                .Pop()
            .Pop();
            return Task.CompletedTask;
        }
    }
}
