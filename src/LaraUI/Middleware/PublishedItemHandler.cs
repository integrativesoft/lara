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
        readonly LaraOptions _options;

        public PublishedItemHandler(RequestDelegate next, LaraOptions options) : base(next)
        {
            _options = options;
        }

        internal override async Task<bool> ProcessRequest(HttpContext http)
        {
            var combined = Published.CombinePathMethod(http.Request.Path, http.Request.Method);
            if (LaraUI.TryGetNode(combined, out var item))
            {
                await item.Run(http, _options);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
