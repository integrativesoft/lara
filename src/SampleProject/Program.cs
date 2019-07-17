/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;

namespace SampleProject
{
    class Program
    {
        static async Task Main()
        {
            // Load classes decorated with Lara attributes
            LaraUI.PublishAssemblies();

            // Start web server
            var host = await LaraUI.StartServer(new StartServerOptions
            {
                Port = 8181,  // alternatively, leave as 0 to assign dynamic port
                AllowLocalhostOnly = true  // accept connection from current machine only (default)
            });

            // Write address in console.
            string address = LaraUI.GetFirstURL(host);
            Console.WriteLine($"Server listening in {address}.");

            // Launch browser tab. Alternatively, comment out and direct the user to localhost:8181.
            LaraUI.LaunchBrowser(address);
            
            // Wait for termination.
            Console.WriteLine("Press Ctrl+C to terminate");
            await host.WaitForShutdownAsync();
        }
    }
}
