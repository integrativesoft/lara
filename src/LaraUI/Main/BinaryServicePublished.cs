/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Middleware;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Integrative.Lara.Main
{
    sealed class BinaryServicePublished : IPublishedItem
    {
        public Func<IBinaryService> Factory { get; }
        public string ContentType { get; }

        public BinaryServicePublished(BinaryServiceContent content)
        {
            Factory = content.GetFactory();
            ContentType = content.ContentType;
        }

        public async Task Run(Application app, HttpContext http, LaraOptions options)
        {
            var context = new WebServiceContext(app, http)
            {
                RequestBody = await MiddlewareCommon.ReadBody(http).ConfigureAwait(false)
            };
            var handler = Factory();
            var data = Array.Empty<byte>();
            if (await MiddlewareCommon.RunHandler(http, async () =>
            {
                data = await handler.Execute();
            }).ConfigureAwait(false))
            {
                await SendReply(context, data);
            }
        }

        private async Task SendReply(WebServiceContext context, byte[] data)
        {
            WebServicePublished.SendHeader(context, ContentType);
            await MiddlewareCommon.WriteBuffer(context.Http, data);
        }
    }
}
