/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Middleware;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Lara middleware class for the ASP.NET Core framework
    /// </summary>
    public sealed class LaraMiddleware
    {
        readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="LaraMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware</param>
        /// <param name="options">Configuration options</param>
        public LaraMiddleware(RequestDelegate next, LaraOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));
            next = new ClientLibraryHandler(next).Invoke;
            next = new PublishedItemHandler(next, options).Invoke;
            next = new DiscardHandler(options.Application, next).Invoke;
            _next = new PostEventHandler(options.Application, next).Invoke;
        }

        /// <summary>
        /// Invokes this middleware.
        /// </summary>
        /// <param name="http">The HttpContext.</param>
        public async Task Invoke(HttpContext http)
        {
            await _next.Invoke(http);
        }
    }
}
