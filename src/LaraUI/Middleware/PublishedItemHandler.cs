/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    sealed class PublishedItemHandler : BaseHandler
    {
        public PublishedItemHandler(RequestDelegate next) : base(next)
        {
        }

        internal override async Task<bool> ProcessRequest(HttpContext http)
        {
            var combined = Published.CombinePathMethod(http.Request.Path, http.Request.Method);
            if (LaraUI.TryGetNode(combined, out var item))
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
