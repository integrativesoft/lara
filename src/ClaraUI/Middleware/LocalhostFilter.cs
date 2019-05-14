/*
Copyright (c) 2019 Integrative Software LLC
Created: 4/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace Integrative.Clara.Middleware
{
    public sealed class LocalhostFilter
    {
        readonly RequestDelegate _next;
        readonly ILogger<LocalhostFilter> _logger;
        readonly IPAddress _ip4, _ip6;

        public LocalhostFilter(RequestDelegate next, ILogger<LocalhostFilter> logger)
        {
            _next = next;
            _logger = logger;
            _ip4 = IPAddress.Parse("127.0.0.1");
            _ip6 = IPAddress.Parse("::1");
        }

        private bool IsLocal(IPAddress address) => address.Equals(_ip4) || address.Equals(_ip6);

        public async Task Invoke(HttpContext context)
        {
            var remote = context.Connection.RemoteIpAddress;
            if (!IsLocal(remote))
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
