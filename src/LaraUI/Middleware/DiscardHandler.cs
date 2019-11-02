/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    sealed class DiscardHandler : BaseHandler
    {
        private const int DiscardDelay = 3000;

        readonly Application _app;

        public DiscardHandler(Application app, RequestDelegate next) : base(next)
        {
            _app = app;
        }

        internal override async Task<bool> ProcessRequest(HttpContext http)
        {
            if (http.Request.Method == "POST"
                && http.Request.Path == "/_discard"
                && DiscardParameters.TryParse(http, out var parameters)
                && MiddlewareCommon.TryFindConnection(_app, http, out var connection))
            {
                await Task.Delay(DiscardDelay);
                await connection.Discard(parameters.DocumentId);
                _app.ClearEmptyConnection(connection);
                return true;
            }
            return false;
        }
    }
}
