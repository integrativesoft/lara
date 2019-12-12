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
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Static content to publish on the web server
    /// </summary>
    public class StaticContent : IPublishedItem
    {
        const float RequiredCompressionFactor = 0.9f;

        readonly byte[] _bytes;

        /// <summary>
        /// Returns the byte array that is sent to clients
        /// </summary>
        /// <returns>byte array</returns>
        public byte[] GetBytes() => _bytes;

        /// <summary>
        /// Gets the 'content-type' HTTP header for the static content
        /// </summary>
        /// <value>
        /// The 'content-type' value for the content.
        /// </value>
        public string ContentType { get; } = string.Empty;

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
        public StaticContent(byte[] bytes, string contentType)
            : this(bytes)
        {
            ContentType = contentType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticContent"/> class.
        /// </summary>
        /// <param name="bytes">The byte array.</param>
        public StaticContent(byte[] bytes)
        {
            bytes = bytes ?? throw new ArgumentNullException(nameof(bytes));
            var compressed = LaraTools.Compress(bytes);
            long required = (int)Math.Floor(bytes.LongLength * RequiredCompressionFactor);
            if (compressed.Length > required)
            {
                _bytes = bytes;
                Compressed = false;
            }
            else
            {
                _bytes = compressed;
                Compressed = true;
            }
            ETag = ComputeETag(bytes);
        }

        /// <summary>
        /// Calculates an ETag value based on the given array of bytes
        /// </summary>
        /// <param name="bytes">Array of bytes</param>
        /// <returns>Calculated ETag</returns>
        public static string ComputeETag(byte[] bytes)
        {
            var hash = ComputeHash(bytes);
            return "\"" + hash.ToString(CultureInfo.InvariantCulture) + "\"";
        }

        /// <summary>
        /// Formats a hash value in the eTag format
        /// </summary>
        /// <param name="hash">Hash value</param>
        /// <returns>Formatted hash value in eTag format</returns>
        public static string FormatETag(int hash)
        {
            return "\"" + hash.ToString(CultureInfo.InvariantCulture) + "\"";
        }

        /// <summary>
        /// Computes a hash value for an array of bytes.
        /// </summary>
        /// <param name="data">Array of bytes</param>
        /// <returns>Calculated hash</returns>
        public static int ComputeHash(params byte[] data)
        {
            data = data ?? throw new ArgumentNullException(nameof(data));
            unchecked
            {
                const int p = 16777619;
                int hash = (int)2166136261;

                for (int i = 0; i < data.Length; i++)
                    hash = (hash ^ data[i]) * p;

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }

        /// <summary>
        /// Public method used by the Lara framework
        /// </summary>
        /// <param name="app">Lara application</param>
        /// <param name="http">Http context</param>
        /// <param name="options">Lara options</param>
        /// <returns>Task</returns>
        public async Task Run(Application app, HttpContext http, LaraOptions options)
        {
            http = http ?? throw new ArgumentNullException(nameof(http));
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
            var headers = http.Response.Headers;
            headers.Add("Content-Type", ContentType);
            headers.Add("Cache-Control", "no-cache");
            headers.Add("ETag", ETag);
            if (Compressed)
            {
                headers.Add("Content-Encoding", "deflate");
            }
            await MiddlewareCommon.WriteBuffer(http, _bytes);
        }
    }
}
