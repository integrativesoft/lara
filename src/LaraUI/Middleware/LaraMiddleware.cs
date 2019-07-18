/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Middleware;
using Microsoft.AspNetCore.Http;
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
        /// <param name="next">The next.</param>
        public LaraMiddleware(RequestDelegate next)
        {
            next = new ClientLibraryHandler(next).Invoke;
            next = new PublishedItemHandler(next).Invoke;
            next = new DiscardHandler(next).Invoke;
            _next = new PostEventHandler(next).Invoke;
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
