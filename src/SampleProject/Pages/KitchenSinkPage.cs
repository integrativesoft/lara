/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using Integrative.Lara;
using SampleProject.Components;
using SampleProject.Common;
using System.Threading.Tasks;

namespace SampleProject.Pages
{
    internal class KitchenSinkPage : IPage
    {
        public Task OnGet()
        {
            var document = LaraUI.Page.Document;

            // This sample application loads the CSS library 'Bootstrap'
            SampleAppBootstrap.AppendTo(document.Head);

            document.Body.AppendChild(new KitchenSinkComponent());

            return Task.CompletedTask;
        }

        public static void PublishMugImage(Application app)
        {
            var assembly = typeof(KitchenSinkPage).Assembly;
            var bytes = Tools.LoadEmbeddedResource(assembly, "SampleProject.Assets.Coffee.svg");
            app.PublishFile("/Coffee.svg", new StaticContent(bytes, "image/svg+xml"));
        }
    }
}
