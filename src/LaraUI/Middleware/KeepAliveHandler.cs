/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    class KeepAliveHandler : BaseHandler
    {
        readonly static Task<bool> TaskFalse = Task.FromResult(false);
        readonly static Task<bool> TaskTrue = Task.FromResult(true);

        public const string EventPrefix = "/_keepAlive";
        public const string AjaxMethod = "POST";

        readonly Application _app;

        public KeepAliveHandler(Application app, RequestDelegate next) : base(next)
        {
            _app = app;
        }

        internal override Task<bool> ProcessRequest(HttpContext http)
        {
            if (!IsMatch(http))
            {
                return TaskFalse;
            }
            TryGetDocument(http, out _);
            return TaskTrue;
        }

        private static bool IsMatch(HttpContext http)
        {
            return http.Request.Path == EventPrefix
                && !http.WebSockets.IsWebSocketRequest
                && http.Request.Method == AjaxMethod;
        }

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
