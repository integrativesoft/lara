/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;
using Integrative.Lara;
using SampleProject.Main;
using SampleProject.Other;

namespace SampleProject.KitchenSink
{
    [LaraPage(Address = "/")]
    internal sealed class KitchenSinkForm : IPage
    {
        public static void PublishImages(Application app)
        {
            // publish static image file of coffee mug
            var assembly = typeof(KitchenSinkForm).Assembly;
            var bytes = Tools.LoadEmbeddedResource(assembly, "SampleProject.Assets.Coffee.svg");
            app.PublishFile("/Coffee.svg", new StaticContent(bytes, "image/svg+xml"));
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
                .AddNode(new CheckboxSample())
                .AddNode(new SelectSample())
                .AddNode(new MultiselectSample().Root)
                .AddNode(new LockingSample().Root)
                .AddNode(new LongRunningSample().Root)
                .Push("div", "mt-3")
                    .Push("div", "mt-2")
                        .Push("a")
                            .Attribute("href", AutocompletePage.Address)
                            .Attribute("target", "_blank")
                            .AppendText("Autocomplete example")
                        .Pop()
                    .Pop()
                    .Push("div", "mt-2")
                        .Push("a")
                            .Attribute("href", UploadFilePage.Address)
                            .Attribute("target", "_blank")
                            .AppendText("File upload example")
                        .Pop()
                    .Pop()
                .Pop()
            .Pop();

            return Task.CompletedTask;
        }
    }
}
