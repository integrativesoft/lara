/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Integrative.Clara.Tools
{
    public static class ServerLauncher
    {
        public static async Task<IWebHost> StartServer()
        {
            return await StartServer(new ServerOptions());
        }

        public static async Task<IWebHost> StartServer(ServerOptions options)
        {
            var host = CreateBrowserHost(options);
            await host.StartAsync();
            return host;
        }

        private static IWebHost CreateBrowserHost(ServerOptions options)
        {
            return new WebHostBuilder()
                .UseKestrel(kestrel => kestrel.Listen(IPAddress.Loopback, options.Port))
                .UseStartup<BrowserAppStartup>()
                .Configure(app =>
                {
                    if (options.AllowLocalhostOnly)
                    {
                        app.UseMiddleware<LocalhostFilter>();
                    }
                    app.UseMiddleware<ClaraMiddleware>();
                    if (options.ShowNotFoundPage)
                    {
                        app.UseMiddleware<NotFoundMiddleware>();
                    }
                })
                .Build();
        }

        public static void LaunchBrowser(string address)
        {
            ClaraTools.LaunchBrowser(address);
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
