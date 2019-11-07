/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using SampleProject.Other;
using System.Threading.Tasks;

namespace SampleProject
{
    [LaraPage(Address = "/")]
    sealed class KitchenSinkForm : IPage
    {
        static KitchenSinkForm()  // static constructor
        {
            // publish static image file of coffee mug
            var assembly = typeof(KitchenSinkForm).Assembly;
            var bytes = Tools.LoadEmbeddedResource(assembly, "SampleProject.Assets.Coffee.svg");
            var app = LaraUI.Context.Application;
            app.PublishFile("/Coffee.svg", new StaticContent(bytes, "image/svg+xml"));
        }

        public KitchenSinkForm()  // instance constructor
        {
        }

        public Task OnGet()  // OnGet method from interface IPage
        {
            var document = LaraUI.Page.Document;

            // This sample application loads the CSS library 'Bootstrap'
            SampleAppBootstrap.AppendTo(document.Head);

            // Load custom controls in document body
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
                            .AppendText("LaraBuilder example")
                        .Pop()
                    .Pop()
                    .Push("div", "mt-2")
                        .Push("a")
                            .Attribute("href", ReactiveSimplePage.PageAddress)
                            .AppendText("Reactive programming example 1")
                        .Pop()
                    .Pop()
                    .Push("div")
                        .Push("a")
                            .Attribute("href", RactiveCollectionPage.PageAddress)
                            .AppendText("Reactive programming example 2")
                        .Pop()
                    .Pop()
                    .Push("div")
                        .Push("a")
                            .Attribute("href", ReactiveFeedPage.PageAddress)
                            .AppendText("Reactive programming example 3")
                        .Pop()
                    .Pop()
                    .Push("div", "mt-2")
                        .Push("a")
                            .Attribute("href", CounterPage.PageAddress)
                            .AppendText("Web component example 1")
                        .Pop()
                    .Pop()
                    .Push("div")
                        .Push("a")
                            .Attribute("href", CheckboxPage.PageAddress)
                            .AppendText("Web component example 2")
                        .Pop()
                    .Pop()
                    .Push("div")
                        .Push("a")
                            .Attribute("href", CardPage.PageAddress)
                            .AppendText("Web component example 3")
                        .Pop()
                    .Pop()
                .Pop()
            .Pop();

            return Task.CompletedTask;
        }
    }
}
