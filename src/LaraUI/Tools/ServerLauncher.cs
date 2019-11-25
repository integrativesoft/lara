/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace Integrative.Lara.Tools
{
    static class ServerLauncher
    {
        public const string ErrorAddress = "/Error";

        public static async Task<IWebHost> StartServer(Application app, StartServerOptions options)
        {
            var host = CreateBrowserHost(app, options);
            if (options.PublishAssembliesOnStart)
            {
                app.PublishAssemblies();
            }
            await host.StartAsync(CancellationToken.None);
            return host;
        }

        private static IWebHost CreateBrowserHost(Application laraApp, StartServerOptions options)
        {
            return new WebHostBuilder()
                .UseKestrel(kestrel => kestrel.Listen(options.IPAddress, options.Port))
                .Configure(app =>
                {
                    ConfigureApp(app, laraApp, options);
                })
                .Build();
        }

        internal static void ConfigureApp(IApplicationBuilder app, Application laraApp, StartServerOptions options)
        {
            ConfigureExceptions(app, options);
            app.UseLara(laraApp, options);
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
