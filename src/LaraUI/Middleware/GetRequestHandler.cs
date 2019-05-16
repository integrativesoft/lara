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
    sealed class GetRequestHandler : BaseHandler
    {
        public GetRequestHandler(RequestDelegate next) : base(next)
        {
        }

        internal override async Task<bool> ProcessRequest(HttpContext http)
        {
            if (http.Request.Method == "GET"
                && LaraUI.TryGetNode(http.Request.Path, out var item))
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
