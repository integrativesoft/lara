/*
Copyright (c) 2019 Integrative Software LLC
Created: 9/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Integrative.Lara.Middleware;
using Integrative.Lara.Tools;
using System;
using System.Collections.Generic;
using System.Net;

namespace Integrative.Lara
{
    /// <summary>
    /// This class defines a set of default error pages
    /// </summary>
    public sealed class ErrorPages
    {
        readonly Dictionary<HttpStatusCode, PagePublished> _map;
        readonly Dictionary<HttpStatusCode, PagePublished> _defaults;

        internal ErrorPages()
        {
            _map = new Dictionary<HttpStatusCode, PagePublished>();
            _defaults = new Dictionary<HttpStatusCode, PagePublished>
            {
                { HttpStatusCode.NotFound, new PagePublished(DefaultNotFound, HttpStatusCode.NotFound) },
                { HttpStatusCode.InternalServerError, new PagePublished(DefaultServerError, HttpStatusCode.InternalServerError) }
            };
        }

        /// <summary>
        /// Defines a default page to show for GET requests with a given error code
        /// </summary>
        /// <param name="code">status code</param>
        /// <param name="factory">handler that creates page</param>
        public void SetDefaultPage(HttpStatusCode code, Func<IPage> factory)
        {
            _map.Remove(code);
            var page = new PagePublished(factory);
            _map.Add(code, page);
        }

        /// <summary>
        /// Removes a default error page associated with a status code
        /// </summary>
        /// <param name="code">status code</param>
        public void Remove(HttpStatusCode code)
        {
            _map.Remove(code);
        }

        internal PagePublished GetPage(HttpStatusCode code)
        {
            TryGetPage(code, out var page);
            return page;
        }

        internal bool TryGetPage(HttpStatusCode code, out PagePublished page)
        {
            return _map.TryGetValue(code, out page)
                || _defaults.TryGetValue(code, out page);
        }

        private IPage DefaultNotFound()
        {
            var url = LaraUI.Context.Http.Request.Path;
            return new DefaultErrorPage
            {
                Title = "Not Found",
                Message = $"The requested URL '{url}' was not found on this server."
            };
        }

        internal IPage DefaultServerError()
        {
            return new DefaultErrorPage
            {
                Title = "Internal Server Error",
                Message = $"The server encountered an internal error or misconfiguration and was unable to complete your request."
            };
        }

        internal void PublishErrorPage()
        {
            var address = ServerLauncher.ErrorAddress;
            var publish = LaraUI.GetPublished();
            var page = new PagePublished(DefaultServerError, HttpStatusCode.InternalServerError);
            publish.Publish(address, page);
            var combined = Published.CombinePathMethod(address, "POST");
            publish.Publish(combined, page);
        }

        internal void PublishErrorImage()
        {
            var address = ServerLauncher.ErrorAddress + ".svg";
            var publish = LaraUI.GetPublished();
            var assembly = typeof(LaraUI).Assembly;
            var bytes = ClientLibraryHandler.LoadFile(assembly, "Integrative.Lara.Assets.Error.svg");
            publish.Publish(address, new StaticContent(bytes, "image/svg+xml"));
        }
    }
}
