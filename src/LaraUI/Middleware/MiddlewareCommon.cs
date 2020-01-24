/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    internal static class MiddlewareCommon
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
            context.Response.Headers.Add("Cache-Control", "max-age=31556926");
        }

        public static void AddHeaderTextHtml(HttpContext http)
        {
            http.Response.Headers.Add("Content-Type", "text/html; charset=utf-8");
        }

        // ReSharper disable once InconsistentNaming
        public static void AddHeaderJSON(HttpContext http)
        {
            http.Response.Headers.Add("Content-Type", "application/json");
        }

        public static bool TryFindConnection(Application app, HttpContext http, [NotNullWhen(true)] out Connection? connection)
        {
            connection = null;
            return http.Request.Cookies.TryGetValue(GlobalConstants.CookieSessionId, out var value)
                && Guid.TryParseExact(value, GlobalConstants.GuidFormat, out var guid)
                && app.TryGetConnection(guid, out connection)
                && connection.RemoteIp.Equals(http.Connection.RemoteIpAddress);
        }

        public static bool TryGetParameter(IQueryCollection query, string name, [NotNullWhen(true)] out string? value)
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

        public static async Task<(bool, T?)>
            ReadWebSocketMessage<T>(WebSocket socket, int maxSize) where T : class
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using var ms = new MemoryStream();
            WebSocketReceiveResult result;
            do
            {
                result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                ms.Write(buffer.Array, buffer.Offset, result.Count);
            }
            while (!result.EndOfMessage && result.Count <= maxSize);
            return ProcessWebSocketMessage<T>(maxSize, ms, result);
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Cannot crash")]
        internal static (bool, T?) ProcessWebSocketMessage<T>(int maxSize,
            MemoryStream ms, WebSocketReceiveResult result) where T : class
        {
            ms.Seek(0, SeekOrigin.Begin);
            if (result.MessageType != WebSocketMessageType.Text)
            {
                return (false, default);
            }
            else if (result.Count > maxSize)
            {
                return (false, default);
            }
            try
            {
                var parameters = LaraTools.Deserialize<T>(ms);
                return (true, parameters);
            }
            catch
            {
                return (false, default);
            }
        }

        public static async Task<string> ReadBody(HttpContext http)
        {
            if (http.Request.Body == null)
            {
                return string.Empty;
            }
            using var reader = new StreamReader(http.Request.Body, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }

        public static async Task<bool> RunHandler(HttpContext http, Func<Task> handler)
        {
            try
            {
                await handler();
                return true; 
            }
            catch (StatusCodeException e)
            {
                await SendStatusReply(http, e.StatusCode, e.Message);
                return false;
            }
        }
    }
}
