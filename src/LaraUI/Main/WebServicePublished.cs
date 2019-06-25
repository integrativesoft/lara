/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Middleware;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Integrative.Lara.Main
{
    sealed class WebServicePublished : IPublishedItem
    {
        public Func<IWebService> Factory { get; }
        public string ContentType { get; }

        public WebServicePublished(WebServiceContent content)
        {
            Factory = content.Factory;
            ContentType = content.ContentType;
        }

        public async Task Run(HttpContext http)
        {
            var context = new WebServiceContext
            {
                Http = http,
                RequestBody = await MiddlewareCommon.ReadBody(http)
            };
            var handler = Factory();
            string data = string.Empty;
            if (await MiddlewareCommon.RunHandler(http, async () =>
            {
                data = await handler.Execute(context);
            }))
            {
                await SendReply(context, data);
            }
        }

        private async Task SendReply(WebServiceContext context, string data)
        {
            var http = context.Http;
            MiddlewareCommon.SetStatusCode(http, context.StatusCode);
            var headers = http.Response.Headers;
            headers.Add("Content-Type", ContentType);
            await MiddlewareCommon.WriteUtf8Buffer(http, data);
        }
    }
}
