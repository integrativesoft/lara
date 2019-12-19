/*
Copyright (c) 2019 Integrative Software LLC
Created: 12/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    [DataContract]
    class FormFile : IFormFile
    {
        [DataMember]
        public string ContentType { get; set; } = string.Empty;

        [DataMember]
        public string ContentDisposition { get; set; } = string.Empty;

        [DataMember]
        public string Name { get; set; } = string.Empty;

        [DataMember]
        public string FileName { get; set; } = string.Empty;

        [DataMember]
        public string Content { get; set; } = string.Empty;

        readonly HeaderDictionary _headers = new HeaderDictionary();
        public IHeaderDictionary Headers => _headers;

        public long Length => Content.Length;

        public void CopyTo(Stream target)
        {
            var bytes = GetBytes();
            target.Write(bytes, 0, bytes.Length);
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            var bytes = GetBytes();
            return target.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
        }

        public Stream OpenReadStream()
        {
            var bytes = GetBytes();
            return new MemoryStream(bytes);
        }

        private byte[] GetBytes()
        {
            return Convert.FromBase64String(Content);
        }
    }
}
