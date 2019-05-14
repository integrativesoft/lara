/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Middleware;
using Integrative.Clara.Tools;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Integrative.Clara.Main
{
    public class StaticContent : IPublishedItem
    {
        const float RequiredCompressionFactor = 0.95f;

        public byte[] Bytes { get; }
        public string ContentType { get; }
        public string ETag { get; }
        public bool Compressed { get; }

        public StaticContent(byte[] bytes, string contentType = ContentTypes.ApplicationOctetStream)
        {
            if (bytes == null)
            {
                throw new NullReferenceException("The parameter 'bytes' cannot be null.");
            }
            var compressed = ClaraTools.Compress(bytes);
            long required = (int)Math.Floor(bytes.LongLength * RequiredCompressionFactor);
            if (compressed.Length > required)
            {
                Bytes = bytes;
                Compressed = false;
            }
            else
            {
                Bytes = compressed;
                Compressed = true;
            }
            ContentType = contentType;
            ETag = GetETag(bytes);
        }

        private static string GetETag(byte[] bytes)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                var hash = Convert.ToBase64String(sha1.ComputeHash(bytes));
                return "\"" + hash + "\""; 
            }
        }

        public async Task Run(HttpContext http)
        {
            if (IsMatchETag(http))
            {
                SendMatchStatus(http);
            }
            else
            {
                await SendContent(http);
            }
        }

        private bool IsMatchETag(HttpContext http)
        {
            if (http.Request.Headers.TryGetValue("If-None-Match", out var values))
            {
                string eTagClient = values[values.Count - 1];
                return ETag == eTagClient;
            }
            else
            {
                return false;
            }
        }

        private void SendMatchStatus(HttpContext http)
        {
            MiddlewareCommon.SetStatusCode(http, HttpStatusCode.NotModified);
        }

        private async Task SendContent(HttpContext http)
        {
            MiddlewareCommon.SetStatusCode(http, HttpStatusCode.OK);
            var headers = http.Response.Headers;
            headers.Add("Content-Type", ContentType);
            headers.Add("ETag", ETag);
            if (Compressed)
            {
                headers.Add("Content-Encoding", "deflate");
            }
            await MiddlewareCommon.WriteBuffer(http, Bytes);
        }
    }
}
