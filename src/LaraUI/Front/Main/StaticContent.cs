/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Integrative.Lara.Middleware;
using Integrative.Lara.Tools;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Static content to publish on the web server
    /// </summary>
    /// <seealso cref="Integrative.Lara.Main.IPublishedItem" />
    public class StaticContent : IPublishedItem
    {
        const float RequiredCompressionFactor = 0.9f;

        /// <summary>
        /// Gets the byte array of the content
        /// </summary>
        /// <value>
        /// The byte array.
        /// </value>
        public byte[] Bytes { get; }

        /// <summary>
        /// Gets the 'content-type' HTTP header for the static content
        /// </summary>
        /// <value>
        /// The 'content-type' value for the content.
        /// </value>
        public string ContentType { get; }

        /// <summary>
        /// Gets the e-Tag value for the content.
        /// </summary>
        /// <value>
        /// The e-Tag value.
        /// </value>
        public string ETag { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="StaticContent"/> is compressed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if compressed; otherwise, <c>false</c>.
        /// </value>
        public bool Compressed { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticContent"/> class.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        /// <param name="contentType">The 'content-type' HTTP header value.</param>
        /// <exception cref="NullReferenceException">The parameter 'bytes' cannot be null.</exception>
        public StaticContent(byte[] bytes, string contentType = ContentTypes.ApplicationOctetStream)
        {
            if (bytes == null)
            {
                throw new NullReferenceException("The parameter 'bytes' cannot be null.");
            }
            var compressed = LaraTools.Compress(bytes);
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

        /// <summary>
        /// Public method used by the Lara framework
        /// </summary>
        /// <param name="http"></param>
        /// <returns></returns>
        public async Task Run(HttpContext http)
        {
            if (IsMatchETag(http.Request.Headers))
            {
                SendMatchStatus(http);
            }
            else
            {
                await SendContent(http);
            }
        }

        private bool IsMatchETag(IHeaderDictionary headers)
        {
            if (headers.TryGetValue("If-None-Match", out var values))
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
