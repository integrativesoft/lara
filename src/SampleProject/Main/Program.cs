/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Threading.Tasks;
using Integrative.Lara;
using SampleProject.KitchenSink;

namespace SampleProject.Main
{
    internal static class Program
    {
        private static async Task Main()
        {
            using var app = new Application();
            KitchenSinkForm.PublishImages(app);
            await app.Start(new StartServerOptions { Port = 8182, PublishAssembliesOnStart = true });
            Console.WriteLine("Listening on http://localhost:8182/");
            LaraUI.LaunchBrowser("http://localhost:8182");
            await app.WaitForShutdown();
        }
    }
}
