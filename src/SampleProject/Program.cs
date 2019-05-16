/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.DOM;
using Integrative.Lara.Main;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;

namespace SampleProject
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Starting...");

            // create home page
            LaraUI.Publish("/", () => new MyPage());

            // start web server
            var host = await LaraUI.StartServer();

            // write address in console
            string address = LaraUI.GetFirstURL(host);
            Console.WriteLine($"Server listening in {address}.");

            // launch browser tab
            LaraUI.LaunchBrowser(address);

            // wait for termination
            Console.WriteLine("Press Ctrl+C to terminate");
            await host.WaitForShutdownAsync();
        }
    }

    class MyPage : IPage
    {
        int counter = 0;

        public Task OnGet(IPageContext context)
        {
            var button = new Element("button");
            var text = new TextNode("Click me");
            button.AppendChild(text);
            button.On("click", app =>
            {
                counter++;
                text.Data = $"Clicked {counter} times";
                return Task.CompletedTask;
            });
            context.Document.Body.AppendChild(button);
            return Task.CompletedTask;
        }
    }
}
