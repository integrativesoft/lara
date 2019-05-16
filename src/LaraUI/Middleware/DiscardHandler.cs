/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    sealed class DiscardHandler : BaseHandler
    {
        static readonly Task<bool> Success = Task.FromResult(true);
        static readonly Task<bool> Failure = Task.FromResult(false);

        public DiscardHandler(RequestDelegate next) : base(next)
        {
        }

        internal override Task<bool> ProcessRequest(HttpContext http)
        {
            if (http.Request.Method == "POST"
                && http.Request.Path == "/_discard"
                && DiscardParameters.TryParse(http, out var parameters)
                && MiddlewareCommon.TryFindConnection(http, out var connection))
            {
                connection.Discard(parameters.DocumentId);
                return Success;
            }
            return Failure;
        }
    }
}
