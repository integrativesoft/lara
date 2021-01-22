/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 12/2019
Author: Pablo Carbonell
*/

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    [DataContract]
    internal class FormFile : IFormFile
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

        private readonly HeaderDictionary _headers = new HeaderDictionary();
        public IHeaderDictionary Headers => _headers;

        [DataMember]
        public long Length { get; set; }

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
