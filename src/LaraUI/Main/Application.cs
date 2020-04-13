/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Represents a hosted Lara application
    /// </summary>
    public sealed class Application : IDisposable
    {
        private readonly Published _published;

        private IModeController? _modeController;

        /// <summary>
        /// Web host instance created after calling the Start() method
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public IWebHost? Host { get; private set; }

        /// <summary>
        /// Defines default error pages
        /// </summary>
        public ErrorPages ErrorPages { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Application()
        {
            _published = new Published();
            ErrorPages = new ErrorPages(_published);
            ErrorPages.PublishErrorImage();
        }

        internal Published GetPublished() => _published;

        /// <summary>
        /// Removes all published addresses
        /// </summary>
        public void ClearAllPublished()
        {
            _published.ClearAll();
        }

        /// <summary>
        /// Disposable implementation
        /// </summary>
        public void Dispose()
        {
            ClearAllPublished();
            _published.Dispose();
            Host?.Dispose();
        }

        #region Publishing

        /// <summary>
        /// Publishes a page.
        /// </summary>
        /// <param name="address">The URL address of the page.</param>
        /// <param name="pageFactory">Handler that creates instances of the page</param>
        public void PublishPage(string address, Func<IPage> pageFactory)
            => _published.Publish(address, new PagePublished(pageFactory));

        /// <summary>
        /// Publishes static content.
        /// </summary>
        /// <param name="address">The URL address of the content.</param>
        /// <param name="content">The static content to be published.</param>
        public void PublishFile(string address, StaticContent content)
            => _published.Publish(address, content);

        /// <summary>
        /// Publishes a web service
        /// </summary>
        /// <param name="content">Web service settings</param>
        public void PublishService(WebServiceContent content)
        {
            content = content ?? throw new ArgumentNullException(nameof(content));
            _published.Publish(content);
        }

        /// <summary>
        /// Publishes a web service
        /// </summary>
        /// <param name="content">Binary web service settings</param>
        public void PublishService(BinaryServiceContent content)
        {
            content = content ?? throw new ArgumentNullException(nameof(content));
            _published.Publish(content);
        }

        /// <summary>
        /// Unpublishes an address and its associated content.
        /// </summary>
        /// <param name="path">The path.</param>
        public void UnPublish(string path)
        {
            _published.UnPublish(path);
        }

        /// <summary>
        /// Publishes all classes marked with the attributes [LaraPage] and [LaraWebService]
        /// </summary>
        public void PublishAssemblies()
            => AssembliesReader.LoadAssemblies(this);

        /// <summary>
        /// Unpublished a web service
        /// </summary>
        /// <param name="address">The URL address of the web service</param>
        /// <param name="method">The HTTP method of the web service</param>
        public void UnPublish(string address, string method)
        {
            address = address ?? throw new ArgumentNullException(nameof(address));
            method = method ?? throw new ArgumentNullException(nameof(method));
            _published.UnPublish(address, method);
        }

        internal bool TryGetNode(string path, out IPublishedItem item)
            => _published.TryGetNode(path, out item);

        internal bool TryGetConnection(Guid guid, out Connection connection)
            => _published.Connections.TryGetConnection(guid, out connection);

        internal Connection CreateConnection(IPAddress remoteIp)
            => GetController().CreateConnection(remoteIp);

        internal Task ClearEmptyConnection(Connection connection)
            => _published.Connections.ClearEmptyConnection(connection);

        private IModeController GetController()
        {
            return _modeController ?? throw new MissingMemberException(nameof(Application), nameof(_modeController));
        }

        #endregion

        #region Web Components

        /// <summary>
        /// Registers a specific web component
        /// </summary>
        /// <param name="options">Web component publish options</param>
        public void PublishComponent(WebComponentOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));
            _published.Publish(options);
        }

        /// <summary>
        /// Unregisters a specific web component
        /// </summary>
        /// <param name="tagName">Tag name to unpublish</param>
        public void UnPublishWebComponent(string tagName)
            => _published.UnPublishWebComponent(tagName);

        internal bool TryGetComponent(string tagName, out Type type)
            => _published.TryGetComponent(tagName, out type);

        #endregion

        #region Server

        /// <summary>
        /// Starts the web server
        /// </summary>
        /// <returns>Task</returns>
        // ReSharper disable once UnusedMember.Global
        public Task Start()
            => Start(new StartServerOptions());

        /// <summary>
        /// Starts the web server. Use with 'await'.
        /// </summary>
        /// <param name="options">The server options.</param>
        /// <returns>Task</returns>
        public async Task Start(StartServerOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));
            Host?.Dispose();
            CreateModeController(options.Mode);
            Host = await GetController().Start(this, options);
        }

        internal void CreateModeController(ApplicationMode mode)
        {
            if (_modeController == null || _modeController.Mode != mode)
            {
                _modeController = ModeControllerFactory.Create(this, mode);
            }
        }

        /// <summary>
        /// Stops the web server gracefully
        /// </summary>
        /// <param name="token">Token to indicate when the stop should not be graceful anymore</param>
        /// <returns>Task</returns>
        public Task Stop(CancellationToken token = default) => GetHost().StopAsync(token);

        /// <summary>
        /// Returns a task that is completed when the server stops
        /// </summary>
        /// <param name="token">Token to trigger shutdown</param>
        /// <returns>Task</returns>
        // ReSharper disable once UnusedMember.Global
        public Task WaitForShutdown(CancellationToken token = default) => Host.WaitForShutdownAsync(token);

        internal IWebHost GetHost()
        {
            return Host ?? throw new MissingMemberException(nameof(Application), nameof(Host));
        }

        #endregion

        #region Application behavior modes

        internal double KeepAliveInterval => GetController().KeepAliveInterval;

        internal bool AllowLocalhostOnly => GetController().LocalhostOnly;

        internal int DiscardDelay => GetController().DiscardDelay;

        #endregion

        #region Testing methods

        internal void SetHost(IWebHost host)
        {
            Host = host;
        }

        #endregion
    }
}
