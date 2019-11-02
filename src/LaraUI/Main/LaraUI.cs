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
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

[assembly: NeutralResourcesLanguage("en-US")]
namespace Integrative.Lara
{
    /// <summary>
    /// The main Lara static class
    /// </summary>
    public static class LaraUI
    {
        #region Default application (obsolete)

        internal static Application DefaultApplication { get; } = new Application();

        /// <summary>
        /// Defines default error pages
        /// </summary>
        public static ErrorPages ErrorPages => DefaultApplication.ErrorPages;

        /// <summary>
        /// Removes all published elements
        /// </summary>
        public static void ClearAll() => DefaultApplication.ClearAllPublished();

        /// <summary>
        /// Publishes a page.
        /// </summary>
        /// <param name="address">The URL address of the page.</param>
        /// <param name="pageFactory">Handler that creates instances of the page</param>
        public static void Publish(string address, Func<IPage> pageFactory)
            => DefaultApplication.PublishPage(address, pageFactory);

        /// <summary>
        /// Publishes static content.
        /// </summary>
        /// <param name="address">The URL address of the content.</param>
        /// <param name="content">The static content to be published.</param>
        public static void Publish(string address, StaticContent content)
            => Context.Application.PublishFile(address, content);

        /// <summary>
        /// Publishes a web service
        /// </summary>
        /// <param name="content">Web service settings</param>
        public static void Publish(WebServiceContent content)
            => Context.Application.PublishService(content);

        /// <summary>
        /// Unpublishes an address and its associated content.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void UnPublish(string path)
            => DefaultApplication.UnPublish(path);

        /// <summary>
        /// Publishes all classes marked with the attributes [LaraPage] and [LaraWebService]
        /// </summary>
        public static void PublishAssemblies()
            => AssembliesReader.LoadAssemblies(DefaultApplication);

        /// <summary>
        /// Unpublished a web service
        /// </summary>
        /// <param name="address">The URL address of the web service</param>
        /// <param name="method">The HTTP method of the web service</param>
        public static void UnPublish(string address, string method)
            => DefaultApplication.UnPublish(address, method);


        /// <summary>
        /// Registers a specific web component
        /// </summary>
        /// <param name="options"></param>
        public static void Publish(WebComponentOptions options)
            => DefaultApplication.PublishComponent(options);

        /// <summary>
        /// Unregisters a specific web component
        /// </summary>
        /// <param name="tagName"></param>
        public static void UnPublishWebComponent(string tagName)
            => DefaultApplication.UnPublishWebComponent(tagName);

        #endregion

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

        #region Tools

        /// <summary>
        /// Starts the web server. Use with 'await'.
        /// </summary>
        public static Task<IWebHost> StartServer()
            => StartServer(new StartServerOptions());

        /// <summary>
        /// Starts the web server. Use with 'await'.
        /// </summary>
        /// <param name="options">The server options.</param>
        public static Task<IWebHost> StartServer(StartServerOptions options)
            => ServerLauncher.StartServer(options);

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1055:Uri return values should not be strings", Justification = "Source of information is string")]
        public static string GetFirstURL(IWebHost host)
        {
            host = host ?? throw new ArgumentNullException(nameof(host));
            return LaraTools.GetFirstUrl(host);
        }

        /// <summary>
        /// JSON tools
        /// </summary>
        public static LaraJson JSON { get; } = new LaraJson();

        #endregion

        #region Internal tools

        internal static bool TryGetComponent(string tagName, out Type type)
            =>   Context.Application.TryGetComponent(tagName, out type);

        #endregion
    }
}
