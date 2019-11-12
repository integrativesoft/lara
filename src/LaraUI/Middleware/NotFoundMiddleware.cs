/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
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
        readonly LaraOptions _options;
        readonly Application _app;

        /// <summary>
        /// Creates an instance of NotFoundMiddleware
        /// </summary>
        /// <param name="next">Next middleware</param>
        /// <param name="app">Lara application</param>
        /// <param name="options">Configuration options</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required by framework")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Required by framework")]
        public NotFoundMiddleware(RequestDelegate next, Application app, LaraOptions options)
        {
            _options = options;
            _app = app;
        }

        /// <summary>
        /// Invokes this middleware
        /// </summary>
        /// <param name="context">The HttpContext.</param>
        /// <returns></returns>
        public Task Invoke(HttpContext context)
        {
            var page = _app.ErrorPages.GetPage(HttpStatusCode.NotFound);
            return page.Run(_app, context, _options);
        }
    }
}
