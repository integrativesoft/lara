/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    internal class BrowserAppController : BaseModeController
    {
        private const double DefaultTimerInterval = 20 * 1000;          // 20 seconds to trigger updates
        private const double DefaultExpireInterval = 60 * 1000;         // 60 seconds to expire

        internal const double BrowserAppKeepAliveInterval = DefaultExpireInterval / 2.5;

        private Connection? _connection;

        public override int DiscardDelay => 200;

        public BrowserAppController(Application app)
            : base(app, ApplicationMode.BrowserApp)
        {
        }

        public override async Task<IWebHost> Start(Application app, StartServerOptions options)
        {
            var connections = app.GetPublished().Connections;
            connections.StaleCollectionInterval = DefaultTimerInterval;
            connections.StaleExpirationInterval = DefaultExpireInterval;
            var host = await base.Start(app, options);
            LaraUI.LaunchBrowser(host);
            return host;
        }

        public override double KeepAliveInterval => BrowserAppKeepAliveInterval;

        public override Connection CreateConnection(IPAddress remoteIp)
        {
            if (!AcceptConnection(remoteIp))
            {
                throw new StatusForbiddenException(Resources.BrowserAppConnectionRejected);
            }
            _connection = base.CreateConnection(remoteIp);
            _connection.Closing.Subscribe(Stop);
            return _connection;
        }

        private Task Stop()
        {
            using var source = new CancellationTokenSource();
            var token = source.Token;
            var tasks = new[]
            {
                App.Stop(token),
                SignalStop(source)
            };
            return Task.WhenAll(tasks);
        }

        private static Task SignalStop(CancellationTokenSource source)
        {
            source.Cancel();
            return Task.CompletedTask;
        }

        private bool AcceptConnection(IPAddress remoteIp)
        {
            return _connection == null && IPAddress.IsLoopback(remoteIp);
        }

        public override bool LocalhostOnly => true;
    }
}
