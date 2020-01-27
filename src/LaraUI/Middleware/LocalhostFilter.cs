/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 4/2019
Author: Pablo Carbonell
*/

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
        private readonly RequestDelegate _next;
        private readonly ILogger<LocalhostFilter> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next">Next middleware delegate</param>
        /// <param name="logger">Logger</param>
        public LocalhostFilter(RequestDelegate next, ILogger<LocalhostFilter> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes this middleware
        /// </summary>
        /// <param name="context">The HttpContext</param>
        /// <returns>Task</returns>
        public Task Invoke(HttpContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));
            var remote = context.Connection.RemoteIpAddress;
            if (!IPAddress.IsLoopback(remote))
            {
                var msg = $"Forbidden request from {remote}";
                _logger.LogInformation(msg);
                return MiddlewareCommon.SendStatusReply(context, HttpStatusCode.Forbidden, Resources.Http403);
            }
            else
            {
                return _next.Invoke(context);
            }
        }
    }
}
