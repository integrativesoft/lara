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
        private const int DiscardDelay = 3000;

        //static readonly Task<bool> Success = Task.FromResult(true);
        //static readonly Task<bool> Failure = Task.FromResult(false);

        public DiscardHandler(RequestDelegate next) : base(next)
        {
        }

        internal override async Task<bool> ProcessRequest(HttpContext http)
        {
            if (http.Request.Method == "POST"
                && http.Request.Path == "/_discard"
                && DiscardParameters.TryParse(http, out var parameters)
                && MiddlewareCommon.TryFindConnection(http, out var connection))
            {
                await Task.Delay(DiscardDelay);
                connection.Discard(parameters.DocumentId);
                return true;
            }
            return false;
        }
    }
}
