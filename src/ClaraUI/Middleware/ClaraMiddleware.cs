/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Integrative.Clara.Middleware
{
    public sealed class ClaraMiddleware
    {
        readonly RequestDelegate _next;

        public ClaraMiddleware(RequestDelegate next)
        {
            next = new ClientLibraryHandler(next).Invoke;
            next = new GetRequestHandler(next).Invoke;
            next = new DiscardHandler(next).Invoke;
            next = new PostEventHandler(next).Invoke;
            _next = next;
        }

        public async Task Invoke(HttpContext http)
        {
            await _next.Invoke(http);
        }
    }
}
