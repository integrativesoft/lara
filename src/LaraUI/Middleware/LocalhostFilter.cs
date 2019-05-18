/*
Copyright (c) 2019 Integrative Software LLC
Created: 4/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    public sealed class LocalhostFilter
    {
        readonly RequestDelegate _next;
        readonly ILogger<LocalhostFilter> _logger;

        public LocalhostFilter(RequestDelegate next, ILogger<LocalhostFilter> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var remote = context.Connection.RemoteIpAddress;
            if (!IPAddress.IsLoopback(remote))
            {
                string msg = $"Forbidden request from {remote}";
                _logger.LogInformation(msg);
                await MiddlewareCommon.SendStatusReply(context, HttpStatusCode.Forbidden, "HTTP 403: Forbidden.");
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
