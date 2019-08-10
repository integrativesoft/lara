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
            BootstrapLoader.AddBootstrap(document.Head);

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
                            .Attribute("href", "/reactor1")
                            .AddTextNode("Ractive programming example 1")
                        .Pop()
                    .Pop()
                    .Push("div")
                        .Push("a")
                            .Attribute("href", "/reactor2")
                            .AddTextNode("Ractive programming example 2")
                        .Pop()
                    .Pop()
                    .Push("div")
                        .Push("a")
                            .Attribute("href", "/reactor3")
                            .AddTextNode("Ractive programming example 3")
                        .Pop()
                    .Pop()
                .Pop();

            return Task.CompletedTask;
        }
    }
}
