/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tests.Middleware;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Xunit;

namespace Integrative.Lara.Tests.Main
{
    public class StaticContentTesting : DummyContextTesting
    {
        static readonly HttpClient _client;

        static StaticContentTesting()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate
            };
            _client = new HttpClient(handler);
        }            

        public StaticContentTesting()
        {
            PublishHelper.PublishIfNeeded();
        }

        [Fact]
        public void AreadyCompressedFileDoesNotGetCompressed()
        {
            var bytes = LoadSampleJPEG();
            var content = new StaticContent(bytes, ContentTypes.ImageJpeg);
            Assert.Same(bytes, content.GetBytes());
            Assert.Equal(ContentTypes.ImageJpeg, content.ContentType);
            Assert.False(content.Compressed);
            Assert.False(string.IsNullOrEmpty(content.ETag));
        }

        private byte[] LoadSampleJPEG()
        {
            return LoadAsset("pexels-photo-248673.jpeg");
        }

        private byte[] LoadCompressibleBMP()
        {
            return LoadAsset("Compressible.bmp");
        }

        private byte[] LoadAsset(string filename)
        {
            return LoadResource($"Integrative.Lara.Tests.Assets.{filename}");
        }

        private byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetAssembly(typeof(StaticContentTesting));
            using Stream resFilestream = assembly.GetManifestResourceStream(filename);
            if (resFilestream == null) return null;
            byte[] ba = new byte[resFilestream.Length];
            resFilestream.Read(ba, 0, ba.Length);
            return ba;
        }

        [Fact]
        public async void RequestWithoutETagReceivesFile()
        {
            var bytes = LoadSampleJPEG();
            var content = new StaticContent(bytes, ContentTypes.ImageJpeg);

            using var host = await LaraUI.StartServer(new StartServerOptions
            {
                Application = _context.Application
            });
            string address = LaraUI.GetFirstURL(host);
            LaraUI.Publish("/", content);
            using var response = await _client.GetAsync(address);
            var downloaded = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.TryGetValues("ETag", out var values));
            Assert.Equal(content.ETag, values.FirstOrDefault());
            Assert.Equal(bytes, downloaded);
        }

        [Fact]
        public async void RequestWrongETagReceivesFile()
        {
            var bytes = LoadSampleJPEG();
            var content = new StaticContent(bytes, ContentTypes.ImageJpeg);

            using var host = await LaraUI.StartServer(new StartServerOptions
            {
                Application = _context.Application
            });
            string address = LaraUI.GetFirstURL(host);
            LaraUI.Publish("/", content);

            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri(address)
            };
            request.Headers.TryAddWithoutValidation("If-None-Match", "lalalalala");

            using var response = await _client.SendAsync(request);
            var downloaded = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.TryGetValues("ETag", out var values));
            Assert.Equal(content.ETag, values.FirstOrDefault());
            Assert.Equal(bytes, downloaded);
        }

        [Fact]
        public async void RequestCorrectETagReceivesNotModified()
        {
            var bytes = LoadSampleJPEG();
            var content = new StaticContent(bytes, ContentTypes.ImageJpeg);

            using var host = await LaraUI.StartServer(new StartServerOptions
            {
                Application = _context.Application
            });
            string address = LaraUI.GetFirstURL(host);
            LaraUI.Publish("/", content);

            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri(address)
            };
            request.Headers.TryAddWithoutValidation("If-None-Match", content.ETag);

            using var response = await _client.SendAsync(request);
            var downloaded = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(HttpStatusCode.NotModified, response.StatusCode);
            Assert.False(response.Headers.Contains("ETag"));
            Assert.Empty(downloaded);
        }

        [Fact]
        public async void CompressibleFileIsSentCompressed()
        {
            var bytes = LoadCompressibleBMP();
            var content = new StaticContent(bytes, "image");

            using var host = await LaraUI.StartServer(new StartServerOptions
            {
                Application = _context.Application
            });
            var address = LaraUI.GetFirstURL(host);
            _context.Application.PublishFile("/", content);
            using var response = await _client.GetAsync(address);
            var downloaded = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.TryGetValues("ETag", out var values));
            Assert.Equal(content.ETag, values.FirstOrDefault());
            Assert.Equal(bytes, downloaded);
        }

    }
}
