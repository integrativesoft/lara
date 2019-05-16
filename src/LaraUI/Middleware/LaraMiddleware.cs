/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    public sealed class LaraMiddleware
    {
        readonly RequestDelegate _next;

        public LaraMiddleware(RequestDelegate next)
        {
            next = new ClientLibraryHandler(next).Invoke;
            next = new GetRequestHandler(next).Invoke;
            next = new DiscardHandler(next).Invoke;
            _next = new PostEventHandler(next).Invoke;
        }

        public async Task Invoke(HttpContext http)
        {
            await _next.Invoke(http);
        }
    }
}
