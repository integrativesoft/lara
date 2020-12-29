/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using SampleProject.Pages;
using System;
using System.Threading.Tasks;

namespace SampleProject.Main
{
    internal static class Program
    {
        private static async Task Main()
        {
            // create application
            using var app = new Application();
            KitchenSinkPage.PublishMugImage(app);
            app.PublishPage("/", () => new KitchenSinkPage());
            app.PublishPage("/upload", () => new UploadFilePage());
            app.PublishPage("/server", () => new ServerEventsPage());

            // start application
            await app.Start(new StartServerOptions { Port = 8182 });
            Console.WriteLine("Listening on http://localhost:8182/");
            LaraUI.LaunchBrowser("http://localhost:8182");

            // wait for shutdown
            await app.WaitForShutdown();
        }
    }
}
