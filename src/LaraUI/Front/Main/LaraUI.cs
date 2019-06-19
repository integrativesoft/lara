/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Integrative.Lara.Tools;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// The main Lara static class
    /// </summary>
    public static class LaraUI
    {
        static readonly Published _published;

        static LaraUI()
        {
            _published = new Published();
        }

        /// <summary>
        /// Removes all published elements
        /// </summary>
        public static void ClearAll()
        {
            _published.ClearAll();
        }

        #region Publishing

        /// <summary>
        /// Publishes a page.
        /// </summary>
        /// <param name="address">The address of the page.</param>
        /// <param name="pageFactory">The page factory.</param>
        public static void Publish(string address, Func<IPage> pageFactory)
            => _published.Publish(address, new PagePublished(pageFactory));

        /// <summary>
        /// Publishes static content.
        /// </summary>
        /// <param name="address">The address of the content.</param>
        /// <param name="content">The static content to be published.</param>
        public static void Publish(string address, StaticContent content)
            => _published.Publish(address, content);

        /// <summary>
        /// Unpublishes an address and its associated content.
        /// </summary>
        /// <param name="path">The path.</param>
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

        /// <summary>
        /// Starts the web server. Use with 'await'.
        /// </summary>
        public static async Task<IWebHost> StartServer()
            => await ServerLauncher.StartServer();

        /// <summary>
        /// Starts the web server. Use with 'await'.
        /// </summary>
        /// <param name="options">The server options.</param>
        public static async Task<IWebHost> StartServer(StartServerOptions options)
            => await ServerLauncher.StartServer(options);

        /// <summary>
        /// Launches the user's default web browser on the specified address.
        /// </summary>
        /// <param name="address">The address.</param>
        public static void LaunchBrowser(string address)
            => ServerLauncher.LaunchBrowser(address);

        /// <summary>
        /// Launches the user's default web browser on the first address of the host passed in parameters.
        /// </summary>
        /// <param name="host">The host.</param>
        public static void LaunchBrowser(IWebHost host)
                    => ServerLauncher.LaunchBrowser(host);

        /// <summary>
        /// Gets the first URL associated with the given host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        public static string GetFirstURL(IWebHost host)
            => ServerLauncher.GetFirstUrl(host);

        #endregion

        #region Methods to help testing

        internal static Published GetPublished() => _published;

        #endregion
    }
}
