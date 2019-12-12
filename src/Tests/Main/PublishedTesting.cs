/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Integrative.Lara.Main;
using Integrative.Lara.Middleware;
using Integrative.Lara.Tests.Middleware;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Integrative.Lara.Tests.Main
{
    public class PublishedTesting : DummyContextTesting
    {
        [Fact]
        public void UnpublishRemoves()
        {
            using var app = new Application();
            using var published = app.GetPublished();
            published.Publish("/coco", new StaticContent(new byte[0]));
            published.Publish("/lala", new StaticContent(new byte[0]));
            app.UnPublish("/coco");
            Assert.True(published.TryGetNode("/lala", out _));
            Assert.False(published.TryGetNode("/coco", out _));
        }

        [Fact]
        public async void RedirectExecutes()
        {
            var http = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            var request = new Mock<HttpRequest>();
            http.Setup(x => x.Response).Returns(response.Object);
            http.Setup(x => x.Request).Returns(request.Object);
            request.Setup(x => x.Method).Returns("GET");
            var page = new MyRedirectPage();
            var document = new Document(page, BaseModeController.DefaultKeepAliveInterval);
            var cx = new Connection(Guid.NewGuid(), IPAddress.Loopback);
            var context = new PageContext(_context.Application, http.Object, cx);
            await page.OnGet();
            await PagePublished.ProcessGetResult(http.Object, document, context, HttpStatusCode.OK);
            response.Verify(x => x.Redirect("https://www.google.com"));
        }

        class MyRedirectPage : IPage
        {
            public Task OnGet()
            {
                LaraUI.Page.Navigation.Replace("https://www.google.com");
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void WebServiceContentType()
        {
            var x = new WebServiceContent();
            Assert.Equal("application/json", x.ContentType);
            Assert.Equal("POST", x.Method);
            x.ContentType = "text/html";
            x.Method = "PUT";
            Assert.Equal("text/html", x.ContentType);
            Assert.Equal("PUT", x.Method);
        }

        [Fact]
        public void LaraStringify()
        {
            var start = new MyClass
            {
                Value = 5
            };
            string json = LaraUI.JSON.Stringify(start);
            bool ok = LaraUI.JSON.TryParse<MyClass>(json, out var result);
            Assert.True(ok);
            Assert.Equal(start.Value, result!.Value);
        }

        [DataContract]
        class MyClass
        {
            [DataMember]
            public int Value { get; set; }
        }

        [Fact]
        public void SessionRemoveValue()
        {
            var guid = Guid.Parse("{A11072B8-7CD4-4D70-821D-C8934ACCD270}");
            var connection = new Connection(guid, IPAddress.Loopback);
            var session = new Session(connection);
            session.SaveValue("mykey", "myvalue");
            session.RemoveValue("mykey");
            Assert.False(session.TryGetValue("mykey", out _));
        }

    }
}
