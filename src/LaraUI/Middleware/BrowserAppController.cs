/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    class BrowserAppController : BaseModeController
    {
        private const double DefaultTimerInterval = 20 * 1000;          // 20 seconds to trigger updates
        private const double DefaultExpireInterval = 60 * 1000;         // 60 seconds to expire

        private const double DefaultKeepAlive = DefaultExpireInterval / 2.5;

        Connection _connection;

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

        public override double KeepAliveInterval => DefaultKeepAlive;

        public override Connection CreateConnection(IPAddress remoteIp)
        {
            if (!AcceptConnection(remoteIp))
            {
                throw new StatusForbiddenException();
            }
            _connection = base.CreateConnection(remoteIp);
            _connection.Closing.Subscribe(() => _app.Stop());
            return _connection;
        }

        private bool AcceptConnection(IPAddress remoteIp)
        {
            if (_connection != null)
            {
                return false;
            }
            else if (IPAddress.IsLoopback(remoteIp))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool LocalhostOnly => true;
    }
}
