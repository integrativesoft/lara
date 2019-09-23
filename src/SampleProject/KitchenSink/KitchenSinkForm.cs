/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    [LaraPage(Address = "/")]
    sealed class KitchenSinkForm : IPage
    {
        public KitchenSinkForm()
        {
        }

        public Task OnGet()
        {
            var document = LaraUI.Page.Document;

            // Load styles library 'bootstrap'
            SampleAppBootstrap.AppendTo(document.Head);

            // Load custom controls in document body.
            var builder = new LaraBuilder(document.Body);
            builder.Push("div", "container p-4")
                .AddNode(new CounterSample().Root)
                .AddNode(new CheckboxSample().Root)
                .AddNode(new SelectSample().Root)
                .AddNode(new MultiselectSample().Root)
                .AddNode(new LockingSample().Root)
                .AddNode(new LongRunningSample().Root)
                .Push("div", "mt-3")
                    .Push("div")
                        .Push("a")
                            .Attribute("href", LaraBuilderExample.PageAddress)
                            .AddTextNode("LaraBuilder example")
                        .Pop()
                    .Pop()
                    .Push("div", "mt-2")
                        .Push("a")
                            .Attribute("href", ReactiveSimplePage.PageAddress)
                            .AddTextNode("Ractive programming example 1")
                        .Pop()
                    .Pop()
                    .Push("div")
                        .Push("a")
                            .Attribute("href", RactiveCollectionPage.PageAddress)
                            .AddTextNode("Ractive programming example 2")
                        .Pop()
                    .Pop()
                    .Push("div")
                        .Push("a")
                            .Attribute("href", ReactiveFeedPage.PageAddress)
                            .AddTextNode("Ractive programming example 3")
                        .Pop()
                    .Pop()
                    .Push("div", "mt-2")
                        .Push("a")
                            .Attribute("href", CounterPage.PageAddress)
                            .AddTextNode("Component example 1")
                        .Pop()
                    .Pop()
                    .Push("div")
                        .Push("a")
                            .Attribute("href", CheckboxPage.PageAddress)
                            .AddTextNode("Component example 2")
                        .Pop()
                    .Pop()
                    .Push("div")
                        .Push("a")
                            .Attribute("href", CardPage.PageAddress)
                            .AddTextNode("Component example 3")
                        .Pop()
                    .Pop()
                .Pop()
            .Pop();

            return Task.CompletedTask;
        }
    }
}
