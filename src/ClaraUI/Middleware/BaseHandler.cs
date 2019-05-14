/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Integrative.Clara.Middleware
{
    abstract class BaseHandler
    {
        readonly RequestDelegate _next;

        public BaseHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext http)
        {
            if (!await ProcessRequest(http))
            {
                await _next.Invoke(http);
            }
        }

        internal abstract Task<bool> ProcessRequest(HttpContext http);
    }
}
