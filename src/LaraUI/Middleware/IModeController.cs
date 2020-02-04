/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Integrative.Lara
{
    internal interface IModeController
    {
        Task<IWebHost> Start(Application app, StartServerOptions options);
        Connection CreateConnection(IPAddress remoteIp);
        double KeepAliveInterval { get; }
        ApplicationMode Mode { get; }
        bool LocalhostOnly { get; }
    }

    internal static class ModeControllerFactory
    {
        public static IModeController Create(Application app, ApplicationMode mode)
        {
            return mode == ApplicationMode.BrowserApp ? new BrowserAppController(app) : new BaseModeController(app, ApplicationMode.Default);
        }
    }

    internal class BaseModeController : IModeController
    {
        public const double DefaultKeepAliveInterval
            = StaleConnectionsCollector.DefaultExpireInterval / 2.5;  // at least 2 message attempts per expire period

        protected readonly Application App;

        public BaseModeController(Application app, ApplicationMode mode)
        {
            App = app;
            Mode = mode;
        }

        public virtual double KeepAliveInterval => DefaultKeepAliveInterval;

        public ApplicationMode Mode { get; }

        public virtual bool LocalhostOnly => false;

        public virtual Connection CreateConnection(IPAddress remoteIp)
        {
            var connections = App.GetPublished().Connections;
            return connections.CreateConnection(remoteIp);
        }

        public virtual Task<IWebHost> Start(Application app, StartServerOptions options)
        {
            return ServerLauncher.StartServer(app, options);
        }
    }
}
