/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
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
            if (options.ShowExceptions)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(ErrorAddress);
                LaraUI.ErrorPages.PublishErrorPage();
            }
            LaraUI.ErrorPages.PublishErrorImage();
            app.UseLara(options);
        }

        public static void LaunchBrowser(string address)
        {
            LaraTools.LaunchBrowser(address);
        }

        public static void LaunchBrowser(IWebHost host)
        {
            string address = GetFirstUrl(host);
            LaunchBrowser(address);
        }

        public static string GetFirstUrl(IWebHost webHost)
        {
            return webHost.ServerFeatures
                .Get<IServerAddressesFeature>()
                .Addresses
                .First();
        }

    }
}
