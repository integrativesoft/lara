/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Integrative.Lara.Middleware;
using Integrative.Lara.Tools;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// The main Lara static class
    /// </summary>
    public static class LaraUI
    {
        static readonly Published _published;

        /// <summary>
        /// Defines default error pages
        /// </summary>
        public static ErrorPages ErrorPages { get; }

        static LaraUI()
        {
            _published = new Published();
            ErrorPages = new ErrorPages();
        }

        /// <summary>
        /// Removes all published elements
        /// </summary>
        public static void ClearAll()
        {
            _published.ClearAll();
        }

        /// <summary>
        /// JSON tools
        /// </summary>
        public static LaraJson JSON { get; } = new LaraJson();

        #region Context variables

        internal static AsyncLocal<ILaraContext> InternalContext { get; } = new AsyncLocal<ILaraContext>();

        /// <summary>
        /// Returns the context associated with the current task. See also 'Page' and 'Service'.
        /// </summary>
        public static ILaraContext Context => InternalContext.Value;

        /// <summary>
        /// Returns the Page context associated the current task, when executing Page events
        /// </summary>
        public static IPageContext Page => InternalContext.Value as IPageContext;

        /// <summary>
        /// Returns the WebService context associated with the current task, when executing web services
        /// </summary>
        public static IWebServiceContext Service => InternalContext.Value as IWebServiceContext;

        #endregion

        #region Publishing

        /// <summary>
        /// Publishes a page.
        /// </summary>
        /// <param name="address">The URL address of the page.</param>
        /// <param name="pageFactory">Handler that creates instances of the page</param>
        public static void Publish(string address, Func<IPage> pageFactory)
            => _published.Publish(address, new PagePublished(pageFactory));

        /// <summary>
        /// Publishes static content.
        /// </summary>
        /// <param name="address">The URL address of the content.</param>
        /// <param name="content">The static content to be published.</param>
        public static void Publish(string address, StaticContent content)
            => _published.Publish(address, content);

        /// <summary>
        /// Publishes a web service
        /// </summary>
        /// <param name="content">Web service settings</param>
        public static void Publish(WebServiceContent content)
            => _published.Publish(content);

        /// <summary>
        /// Unpublishes an address and its associated content.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void UnPublish(string path)
            => _published.UnPublish(path);

        /// <summary>
        /// Publishes all classes marked with the attributes [LaraPage] and [LaraWebService]
        /// </summary>
        public static void PublishAssemblies() => AssembliesReader.LoadAssemblies();

        /// <summary>
        /// Unpublished a web service
        /// </summary>
        /// <param name="address">The URL address of the web service</param>
        /// <param name="method">The HTTP method of the web service</param>
        public static void UnPublish(string address, string method)
            => _published.UnPublish(address, method);

        internal static bool TryGetNode(string path, out IPublishedItem item)
            => _published.TryGetNode(path, out item);

        internal static bool TryGetConnection(Guid guid, out Connection connection)
            => _published.Connections.TryGetConnection(guid, out connection);

        internal static Connection CreateConnection(IPAddress remoteIp)
            => _published.Connections.CreateConnection(remoteIp);

        internal static void ClearEmptyConnection(Connection connection)
            => _published.Connections.ClearEmptyConnection(connection);

        #endregion

        #region Web Components

        /// <summary>
        /// Registers a specific web component
        /// </summary>
        /// <param name="options"></param>
        public static void Publish(WebComponentOptions options)
            => _published.Publish(options);

        /// <summary>
        /// Unregisters a specific web component
        /// </summary>
        /// <param name="tagName"></param>
        public static void UnPublishWebComponent(string tagName)
            => _published.UnPublishWebComponent(tagName);

        internal static bool TryGetComponent(string tagName, out Type type)
            => _published.TryGetComponent(tagName, out type);

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
            => LaraTools.LaunchBrowser(address);

        /// <summary>
        /// Launches the user's default web browser on the first address of the host passed in parameters.
        /// </summary>
        /// <param name="host">The host.</param>
        public static void LaunchBrowser(IWebHost host)
                    => LaraTools.LaunchBrowser(host);

        /// <summary>
        /// Gets the first URL associated with the given host.
        /// </summary>
        /// <param name="host">The host.</param>
        /// <returns></returns>
        public static string GetFirstURL(IWebHost host)
            => LaraTools.GetFirstUrl(host);

        #endregion

        #region Methods to help testing

        internal static Published GetPublished() => _published;

        #endregion
    }
}
