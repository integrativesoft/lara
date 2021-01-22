/*
Copyright (c) 2019-2021 Integrative Software LLC
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
        private static readonly HttpClient _Client;

        static StaticContentTesting()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate
            };
            _Client = new HttpClient(handler);
        }            

        [Fact]
        public void AreadyCompressedFileDoesNotGetCompressed()
        {
            var bytes = LoadSampleJpeg();
            var content = new StaticContent(bytes, ContentTypes.ImageJpeg);
            Assert.Same(bytes, content.GetBytes());
            Assert.Equal(ContentTypes.ImageJpeg, content.ContentType);
            Assert.False(content.Compressed);
            Assert.False(string.IsNullOrEmpty(content.ETag));
        }

        private static byte[] LoadSampleJpeg()
        {
            return LoadAsset("pexels-photo-248673.jpeg");
        }

        private static byte[] LoadCompressibleBmp()
        {
            return LoadAsset("Compressible.bmp");
        }

        private static byte[] LoadAsset(string filename)
        {
            return LoadResource($"Integrative.Lara.Tests.Assets.{filename}");
        }

        private static byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetAssembly(typeof(StaticContentTesting));
            using Stream? resFilestream = assembly!.GetManifestResourceStream(filename);
            if (resFilestream == null) return Array.Empty<byte>();
            byte[] ba = new byte[resFilestream.Length];
            resFilestream.Read(ba, 0, ba.Length);
            return ba;
        }

        [Fact]
        public async void RequestWithoutETagReceivesFile()
        {
            var bytes = LoadSampleJpeg();
            var content = new StaticContent(bytes, ContentTypes.ImageJpeg);

            await Context.Application.Start();
            var address = LaraUI.GetFirstURL(Context.Application.GetHost());
            Context.Application.PublishFile("/", content);
            using var response = await _Client.GetAsync(address);
            var downloaded = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.TryGetValues("ETag", out var values));
            Assert.Equal(content.ETag, values?.FirstOrDefault());
            Assert.Equal(bytes, downloaded);
        }

        [Fact]
        public async void RequestWrongETagReceivesFile()
        {
            var bytes = LoadSampleJpeg();
            var content = new StaticContent(bytes, ContentTypes.ImageJpeg);

            await Context.Application.Start(new StartServerOptions
            {
                AllowLocalhostOnly = true
            });
            var address = LaraUI.GetFirstURL(Context.Application.GetHost());
            Context.Application.PublishFile("/", content);

            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri(address)
            };
            request.Headers.TryAddWithoutValidation("If-None-Match", "lalalalala");

            using var response = await _Client.SendAsync(request);
            var downloaded = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.TryGetValues("ETag", out var values));
            Assert.Equal(content.ETag, values?.FirstOrDefault());
            Assert.Equal(bytes, downloaded);
        }

        [Fact]
        public async void RequestCorrectETagReceivesNotModified()
        {
            var bytes = LoadSampleJpeg();
            var content = new StaticContent(bytes, ContentTypes.ImageJpeg);

            await Context.Application.Start();
            var address = LaraUI.GetFirstURL(Context.Application.GetHost());
            Context.Application.PublishFile("/", content);

            var request = new HttpRequestMessage
            {
                Method = new HttpMethod("GET"),
                RequestUri = new Uri(address)
            };
            request.Headers.TryAddWithoutValidation("If-None-Match", content.ETag);

            using var response = await _Client.SendAsync(request);
            var downloaded = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(HttpStatusCode.NotModified, response.StatusCode);
            Assert.False(response.Headers.Contains("ETag"));
            Assert.Empty(downloaded);
        }

        [Fact]
        public async void CompressibleFileIsSentCompressed()
        {
            var bytes = LoadCompressibleBmp();
            var content = new StaticContent(bytes, "image");

            await Context.Application.Start();
            var address = LaraUI.GetFirstURL(Context.Application.GetHost());
            Context.Application.PublishFile("/", content);
            using var response = await _Client.GetAsync(address);
            var downloaded = await response.Content.ReadAsByteArrayAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(response.Headers.TryGetValues("ETag", out var values));
            Assert.Equal(content.ETag, values?.FirstOrDefault());
            Assert.Equal(bytes, downloaded);
        }

        [Fact]
        public async void ContentNotFound()
        {
            await Context.Application.Start();
            var address = LaraUI.GetFirstURL(Context.Application.GetHost());
            using var response = await _Client.GetAsync(address + "/lalala");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
