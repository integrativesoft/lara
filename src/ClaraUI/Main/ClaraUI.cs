/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Tools;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Integrative.Clara.Main
{
    public static class ClaraUI
    {
        static readonly Published _published;

        static ClaraUI()
        {
            _published = new Published();
        }

        public static void Restart()
        {
            _published.ClearAll();
        }

        #region Publishing

        public static void Publish(string path, Func<BasePage> pageFactory)
            => _published.Publish(path, new PagePublished(pageFactory));

        public static void Publish(string path, StaticContent content)
            => _published.Publish(path, content);

        public static void UnPublish(string path)
            => _published.UnPublish(path);

        internal static bool TryGetNode(string path, out IPublishedItem item)
            => _published.TryGetNode(path, out item);

        internal static bool TryGetConnection(Guid guid, out Connection connection)
            => _published.Connections.TryGetConnection(guid, out connection);

        internal static Connection CreateConnection(IPAddress remoteIp)
            => _published.Connections.CreateConnection(remoteIp);

        #endregion

        #region Launching server and browser

        public static async Task<IWebHost> StartServer()
            => await ServerLauncher.StartServer();

        public static async Task<IWebHost> StartServer(ServerOptions options)
            => await ServerLauncher.StartServer(options);

        public static void LaunchBrowser(string address)
            => ServerLauncher.LaunchBrowser(address);

        public static void LaunchBrowser(IWebHost host)
            => ServerLauncher.LaunchBrowser(host);

        public static string GetFirstURL(IWebHost host)
            => ServerLauncher.GetFirstUrl(host);

        #endregion
    }
}
