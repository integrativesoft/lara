/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Main;
using Integrative.Clara.Tools;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Integrative.Clara.Middleware
{
    sealed class EventParameters
    {
        public Guid DocumentId { get; set; }
        public string ElementId { get; set; }
        public string EventName { get; set; }
        public ClientEventMessage Message { get; set; }

        public static bool TryParse(HttpContext http, out EventParameters parameters)
        {
            var query = http.Request.Query;
            if (MiddlewareCommon.TryGetParameter(query, "doc", out var documentText)
                && MiddlewareCommon.TryGetParameter(query, "el", out var elementId)
                && MiddlewareCommon.TryGetParameter(query, "ev", out var eventName)
                && Guid.TryParseExact(documentText, GlobalConstants.GuidFormat, out var documentId))
            {
                parameters = new EventParameters
                {
                    DocumentId = documentId,
                    ElementId = elementId,
                    EventName = eventName
                };
                return true;
            }
            else
            {
                parameters = default;
                return false;
            }
        }

        public async Task ReadMessage(HttpContext http)
        {
            string body = await ReadBody(http);
            Message = ClaraTools.Deserialize<ClientEventMessage>(body);
        }

        private async Task<string> ReadBody(HttpContext http)
        {
            if (http.Request.Body == null)
            {
                return string.Empty;
            }
            using (var reader = new StreamReader(http.Request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
