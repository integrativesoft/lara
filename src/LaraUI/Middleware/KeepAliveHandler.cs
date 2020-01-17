/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    internal class KeepAliveHandler : BaseHandler
    {
        private static readonly Task<bool> _taskFalse = Task.FromResult(false);
        private static readonly Task<bool> _taskTrue = Task.FromResult(true);

        private const string EventPrefix = "/_keepAlive";
        private const string AjaxMethod = "POST";

        private readonly Application _app;

        public KeepAliveHandler(Application app, RequestDelegate next) : base(next)
        {
            _app = app;
        }

        internal override Task<bool> ProcessRequest(HttpContext http)
        {
            if (!IsMatch(http))
            {
                return _taskFalse;
            }
            TryGetDocument(http, out _);
            return _taskTrue;
        }

        private static bool IsMatch(HttpContext http)
        {
            return http.Request.Path == EventPrefix
                && !http.WebSockets.IsWebSocketRequest
                && http.Request.Method == AjaxMethod;
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool TryGetDocument(HttpContext http, [NotNullWhen(true)] out Document? document)
        {
            document = default;
            return MiddlewareCommon.TryGetParameter(http.Request.Query, "doc", out var text)
                && Guid.TryParseExact(text, GlobalConstants.GuidFormat, out var documentId)
                && MiddlewareCommon.TryFindConnection(_app, http, out var connection)
                && connection.TryGetDocument(documentId, out document);
        }
    }
}
