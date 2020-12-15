/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// A middleware to show a simple 'not found' page
    /// </summary>
    public class NotFoundMiddleware
    {
        private readonly LaraOptions _options;
        private readonly Application _app;

        /// <summary>
        /// Creates an instance of NotFoundMiddleware
        /// </summary>
        /// <param name="next">Next middleware</param>
        /// <param name="app">Lara application</param>
        /// <param name="options">Configuration options</param>
        // ReSharper disable once UnusedParameter.Local
        public NotFoundMiddleware(RequestDelegate next, Application app, LaraOptions options)
        {
            _options = options;
            _app = app;
        }

        /// <summary>
        /// Invokes this middleware
        /// </summary>
        /// <param name="context">The HttpContext.</param>
        /// <returns>Task</returns>
        public Task Invoke(HttpContext context)
        {
            var page = _app.ErrorPages.GetPage(HttpStatusCode.NotFound);
            return page.Run(_app, context, _options);
        }
    }
}
