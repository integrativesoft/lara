/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Threading.Tasks;

namespace Integrative.Lara.Tools
{
    static class ServerLauncher
    {
        public const string ErrorAddress = "/Error";

        public static async Task<IWebHost> StartServer()
        {
            return await StartServer(new StartServerOptions());
        }

        public static async Task<IWebHost> StartServer(StartServerOptions options)
        {
            var host = CreateBrowserHost(options);
            await host.StartAsync();
            return host;
        }

        private static IWebHost CreateBrowserHost(StartServerOptions options)
        {
            return new WebHostBuilder()
                .UseKestrel(kestrel => kestrel.Listen(IPAddress.Loopback, options.Port))
                .Configure(app =>
                {
                    ConfigureApp(app, options);
                })
                .Build();
        }

        internal static void ConfigureApp(IApplicationBuilder app, StartServerOptions options)
        {
            ConfigureExceptions(app, options);
            LaraUI.ErrorPages.PublishErrorImage();
            app.UseLara(options);
        }

        internal static void ConfigureExceptions(IApplicationBuilder app, StartServerOptions options)
        {
            if (options.ShowExceptions)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(ErrorAddress);
                LaraUI.ErrorPages.PublishErrorPage();
            }
        }
    }
}
