/*
Copyright (c) 2019 Integrative Software LLC
Created: 4/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// A middleware class to allow requests from localhost only.
    /// </summary>
    public sealed class LocalhostFilter
    {
        readonly RequestDelegate _next;
        readonly ILogger<LocalhostFilter> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalhostFilter"/> class.
        /// </summary>
        /// <param name="next">The next request delegate.</param>
        /// <param name="logger">The logger instance.</param>
        public LocalhostFilter(RequestDelegate next, ILogger<LocalhostFilter> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes this middleware
        /// </summary>
        /// <param name="context">The HttpContext.</param>
        public async Task Invoke(HttpContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            var remote = context.Connection.RemoteIpAddress;
            if (!IPAddress.IsLoopback(remote))
            {
                string msg = $"Forbidden request from {remote}";
                _logger.LogInformation(msg);
                await MiddlewareCommon.SendStatusReply(context, HttpStatusCode.Forbidden, Resources.Http403);
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
