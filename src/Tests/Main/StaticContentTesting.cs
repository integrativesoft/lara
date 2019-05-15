/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Main;
using Integrative.Clara.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace Integrative.Clara.Tests.Main
{
    public class StaticContentTesting
    {
        [Fact]
        public void AreadyCompressedFileDoesNotGetCompressed()
        {
            var bytes = LoadSampleJPEG();
            var content = new StaticContent(bytes, ContentTypes.ImageJpeg);
            Assert.Same(bytes, content.Bytes);
            Assert.Equal(ContentTypes.ImageJpeg, content.ContentType);
            Assert.False(content.Compressed);
            Assert.False(string.IsNullOrEmpty(content.ETag));
        }

        private byte[] LoadSampleJPEG()
        {
            return LoadAsset("pexels-photo-248673.jpeg");
        }

        private byte[] LoadAsset(string filename)
        {
            return LoadResource($"Integrative.Clara.Tests.Assets.{filename}");
        }

        private byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetAssembly(typeof(StaticContentTesting));
            using (Stream resFilestream = assembly.GetManifestResourceStream(filename))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }

        [Fact]
        public void NullBytesThrowException()
        {
            Assert.Throws<NullReferenceException>(() => new StaticContent(null));
        }

        [Fact]
        public async void RequestWithoutETagReceivesFile()
        {
            var bytes = LoadSampleJPEG();
            var content = new StaticContent(bytes, ContentTypes.ImageJpeg);

            // create mock
            var responseHeaders = new Mock<HeaderDictionary>();
            var body = new Mock<Stream>();
            var response = new Mock<HttpResponse>();
            response.Setup(x => x.Headers).Returns(responseHeaders.Object);
            response.Setup(x => x.Body).Returns(body.Object);
            var mock = new Mock<HttpContext>();
            StringValues values;
            mock.Setup(x => x.Request.Headers.TryGetValue("If-None-Match", out values)).Returns(false);
            mock.Setup(x => x.Response).Returns(response.Object);

            // execute
            await content.Run(mock.Object);

            // verify

        }

        [Fact]
        public void RequestWrongETagReceivesFile()
        {

        }

        [Fact]
        public void RequestCorrectETagReceivesNotModified()
        {

        }

        [Fact]
        public void CompressibleFileIsSentCompressed()
        {

        }

    }
}
