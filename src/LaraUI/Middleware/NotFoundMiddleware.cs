﻿/*
Copyright (c) 2019 Integrative Software LLC
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
        readonly LaraOptions _options;

        /// <summary>
        /// Creates an instance of NotFoundMiddleware
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options">Lara configuration options</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required by framework")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Required by framework")]
        public NotFoundMiddleware(RequestDelegate next, LaraOptions options)
        {
            _options = options;
        }

        /// <summary>
        /// Invokes this middleware
        /// </summary>
        /// <param name="context">The HttpContext.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var page = LaraUI.ErrorPages.GetPage(HttpStatusCode.NotFound);
            await page.Run(context, _options);
        }
    }
}
