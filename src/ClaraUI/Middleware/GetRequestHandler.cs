/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Main;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Integrative.Clara.Middleware
{
    sealed class GetRequestHandler : BaseHandler
    {
        public GetRequestHandler(RequestDelegate next) : base(next)
        {
        }

        internal override async Task<bool> ProcessRequest(HttpContext http)
        {
            if (http.Request.Method == "GET"
                && ClaraUI.TryGetNode(http.Request.Path, out var item))
            {
                await item.Run(http);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
