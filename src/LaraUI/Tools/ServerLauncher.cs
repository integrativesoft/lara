/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Integrative.Lara
{
    internal static class ServerLauncher
    {
        public const string ErrorAddress = "/Error";

        public static async Task<IWebHost> StartServer(Application app, StartServerOptions options)
        {
            var host = CreateBrowserHost(app, options);
            await host.StartAsync(CancellationToken.None);
            return host;
        }

        private static IWebHost CreateBrowserHost(Application laraApp, StartServerOptions options)
        {
            var address = options.IPAddress;
            var port = options.Port;
            var builder = new WebHostBuilder()
                .UseKestrel(kestrel => kestrel.Listen(address, port))
                .Configure(app =>
                {
                    ConfigureApp(app, laraApp, options);
                });
            options.AdditionalConfiguration?.Invoke(builder);
            return builder.Build();
        }

        private static void ConfigureApp(IApplicationBuilder app, Application laraApp, StartServerOptions options)
        {
            ConfigureExceptions(app, laraApp, options);
            app.UseLara(laraApp, options);
        }

        internal static void ConfigureExceptions(IApplicationBuilder app, Application laraApp, StartServerOptions options)
        {
            if (options.ShowExceptions)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(ErrorAddress);
                laraApp.ErrorPages.PublishErrorPage();
            }
        }
    }
}
