/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.DOM;
using Integrative.Clara.Main;
using Integrative.Clara.Tools;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace Integrative.Clara.Middleware
{
    sealed class PostEventHandler : BaseHandler
    {
        public PostEventHandler(RequestDelegate next) : base(next)
        {
        }

        internal override async Task<bool> ProcessRequest(HttpContext http)
        {
            if (http.Request.Method == "POST"
                && http.Request.Path == "/_event"
                && EventParameters.TryParse(http, out var parameters)
                && MiddlewareCommon.TryFindConnection(http, out var connection)
                && connection.TryGetDocument(parameters.DocumentId, out var document)
                && document.TryGetElementById(parameters.ElementId, out var element))
            {
                await parameters.ReadMessage(http);
                using (var access = await document.Semaphore.UseWaitAsync())
                {
                    var context = new ExecutionContext(http, document);
                    ProcessMessageIfNeeded(document, parameters);
                    try
                    {
                        await element.NotifyEvent(parameters.EventName, context);
                    }
                    catch
                    {
                        await MiddlewareCommon.SendStatusReply(http,
                            HttpStatusCode.InternalServerError, "Internal server error");
                        throw;
                    }
                    string queue = document.FlushQueue();
                    await SendReply(http, queue);
                }
                return true;
            }
            return false;
        }

        private void ProcessMessageIfNeeded(Document document, EventParameters parameters)
        {
            var message = parameters.Message;
            if (message != null && message.Values != null)
            {
                ProcessMessage(document, message);
            }
        }

        private void ProcessMessage(Document document, ClientEventMessage message)
        {
            foreach (var row in message.Values)
            {
                if (document.TryGetElementById(row.ElementId, out var element))
                {
                    element.NotifyValue(row.Value);
                }
            }
        }

        private async Task SendReply(HttpContext http, string json)
        {
            MiddlewareCommon.SetStatusCode(http, HttpStatusCode.OK);
            MiddlewareCommon.AddHeaderJSON(http);
            MiddlewareCommon.AddHeaderPreventCaching(http);
            await MiddlewareCommon.WriteUtf8Buffer(http, json);
        }
    }
}
