/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    internal sealed class DiscardHandler : BaseHandler
    {
        private readonly Application _app;

        public DiscardHandler(Application app, RequestDelegate next) : base(next)
        {
            _app = app;
        }

        internal override async Task<bool> ProcessRequest(HttpContext http)
        {
            if (http.Request.Method != "POST" || http.Request.Path != "/_discard" ||
                !DiscardParameters.TryParse(http, out var parameters) ||
                !MiddlewareCommon.TryFindConnection(_app, http, out var connection)) return false;
            await Task.Delay(_app.DiscardDelay);
            await connection.Discard(parameters.DocumentId);
            await _app.ClearEmptyConnection(connection);
            return true;
        }
    }
}
