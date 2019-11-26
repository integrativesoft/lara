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

        public async Task Run(Application app, HttpContext http, LaraOptions options)
        {
            var context = new WebServiceContext(app, http)
            {
                RequestBody = await MiddlewareCommon.ReadBody(http).ConfigureAwait(false)
            };
            var handler = Factory();
            var data = string.Empty;
            if (await MiddlewareCommon.RunHandler(http, async () =>
            {
                data = await handler.Execute();
            }).ConfigureAwait(false))
            {
                await SendReply(context, data);
            }
        }

        private async Task SendReply(WebServiceContext context, string data)
        {
            SendHeader(context, ContentType);
            await MiddlewareCommon.WriteUtf8Buffer(context.Http, data);
        }

        internal static void SendHeader(WebServiceContext context, string contentType)
        {
            var http = context.Http;
            var headers = http.Response.Headers;
            if (!string.IsNullOrEmpty(contentType))
            {
                headers.Add("Content-Type", contentType);
            }
        }
    }
}
