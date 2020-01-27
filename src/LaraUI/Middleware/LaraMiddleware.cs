/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

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
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="LaraMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware</param>
        /// <param name="app">Lara application</param>
        /// <param name="options">Configuration options</param>
        public LaraMiddleware(RequestDelegate next, Application app, LaraOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));
            next = new ClientLibraryHandler(next).Invoke;
            next = new PublishedItemHandler(next, app, options).Invoke;
            next = new DiscardHandler(app, next).Invoke;
            next = new KeepAliveHandler(app, next).Invoke;
            _next = new PostEventHandler(app, next).Invoke;
        }

        /// <summary>
        /// Invokes this middleware.
        /// </summary>
        /// <param name="http">The HttpContext.</param>
        /// <returns>Task</returns>
        public async Task Invoke(HttpContext http)
        {
            await _next.Invoke(http);
        }
    }
}
