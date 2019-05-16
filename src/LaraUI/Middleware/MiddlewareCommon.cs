/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    static class MiddlewareCommon
    {
        public static async Task SendStatusReply(HttpContext context, HttpStatusCode code, string text)
        {
            SetStatusCode(context, code);
            AddHeaderPreventCaching(context);
            await WriteUtf8Buffer(context, text);
        }

        public static async Task WriteUtf8Buffer(HttpContext http, string text)
        {
            var buffer = Encoding.UTF8.GetBytes(text);
            await WriteBuffer(http, buffer);
        }

        public static async Task WriteBuffer(HttpContext http, byte[] buffer)
        {
            await http.Response.Body.WriteAsync(buffer, 0, buffer.Length);
            http.Response.ContentLength = buffer.Length;
        }

        public static void SetStatusCode(HttpContext http, HttpStatusCode code)
        {
            http.Response.StatusCode = (int)code;
        }

        public static void AddHeaderPreventCaching(HttpContext context)
        {
            context.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
        }

        public static void AddHeaderNeverExpires(HttpContext context)
        {
            context.Request.Headers.Add("Cache-Control", "max-age=31556926");
        }

        public static void AddHeaderTextHtml(HttpContext http)
        {
            http.Response.Headers.Add("Content-Type", "text/html; charset=utf-8");
        }

        public static void AddHeaderJSON(HttpContext http)
        {
            http.Response.Headers.Add("Content-Type", "application/json");
        }

        public static bool TryFindConnection(HttpContext http, out Connection connection)
        {
            connection = null;
            return http.Request.Cookies.TryGetValue(GlobalConstants.CookieSessionId, out string value)
                && Guid.TryParseExact(value, GlobalConstants.GuidFormat, out var guid)
                && LaraUI.TryGetConnection(guid, out connection)
                && connection.RemoteIP.Equals(http.Connection.RemoteIpAddress);
        }

        public static bool TryGetParameter(IQueryCollection query, string name, out string value)
        {
            if (query.TryGetValue(name, out var values)
                && values.Count > 0)
            {
                value = values[0];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}
