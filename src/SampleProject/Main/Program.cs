/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    class Program
    {
        static async Task Main()
        {
            using var app = new Application();
            await app.Start(new StartServerOptions
            {
                Mode = ApplicationMode.BrowserApp,   // launches web browser and terminates when closed
                PublishAssembliesOnStart = true,     // searches for classes with 'Lara' attributes
            });
            await app.WaitForShutdown();
        }
    }
}
