/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Integrative.Lara.Middleware;
using Integrative.Lara.Tests.DOM;
using Integrative.Lara.Tests.Main;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using Xunit;

namespace Integrative.Lara.Tests.Middleware
{
    public class MiddlewareTesting
    {
        [Fact]
        public void TryParseMissingDocFails()
        {
            StringValues values;
            var mock = new Mock<IQueryCollection>();
            mock.Setup(x => x.TryGetValue("doc", out values)).Returns(false);
            Assert.False(DiscardParameters.TryParse(mock.Object, out _));
            Assert.False(EventParameters.TryParse(mock.Object, out _));
        }

        [Fact]
        public async void LocalhostFilterTesting()
        {
            var logger = new Mock<ILogger<LocalhostFilter>>();
            var filter = new LocalhostFilter(null, logger.Object);

            var connectionInfo = new Mock<ConnectionInfo>();
            connectionInfo.Setup(x => x.RemoteIpAddress).Returns(IPAddress.Parse("172.217.4.206"));
            var http = new Mock<HttpContext>();
            http.Setup(x => x.Connection).Returns(connectionInfo.Object);
            var response = new Mock<HttpResponse>();
            http.Setup(x => x.Response).Returns(response.Object);
            response.SetupProperty(x => x.StatusCode);
            var headers = new Mock<IHeaderDictionary>();
            response.Setup(x => x.Headers).Returns(headers.Object);
            var body = new Mock<Stream>();
            response.Setup(x => x.Body).Returns(body.Object);
            response.SetupProperty(x => x.ContentLength);

            await filter.Invoke(http.Object);

            Assert.Equal(403, response.Object.StatusCode);
        }

        [Fact]
        public async void NullBodyReturnsEmptyString()
        {
            var http = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            http.Setup(x => x.Request).Returns(request.Object);
            var result = await MiddlewareCommon.ReadBody(http.Object);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void AddHeaderNeverExpires()
        {
            var http = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            http.Setup(x => x.Response).Returns(response.Object);
            var headers = new Mock<IHeaderDictionary>();
            response.Setup(x => x.Headers).Returns(headers.Object);
            MiddlewareCommon.AddHeaderNeverExpires(http.Object);
            headers.Verify(x => x.Add("Cache-Control", "max-age=31556926"), Times.Exactly(1));
        }

        [Fact]
        public void ProcessMessageSkipsEmptyMessage()
        {
            var http = new Mock<HttpContext>();
            var document = new Document(new MyPage());
            var context = new PageContext(http.Object, null, document);
            var parameters = new EventParameters();
            PostEventHandler.ProcessMessageIfNeeded(context, parameters);
        }

        [Fact]
        public async void ConnectionNotFoundSendsReload()
        {
            var http = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var cookies = new Mock<IRequestCookieCollection>();
            var response = new Mock<HttpResponse>();
            var headers = new Mock<IHeaderDictionary>();
            var body = new Mock<Stream>();
            var sockets = new Mock<WebSocketManager>();
            http.Setup(x => x.Request).Returns(request.Object);
            http.Setup(x => x.Response).Returns(response.Object);
            http.Setup(x => x.WebSockets).Returns(sockets.Object);
            request.Setup(x => x.Method).Returns("POST");
            request.Setup(x => x.Path).Returns("/_event");
            request.Setup(x => x.Cookies).Returns(cookies.Object);
            response.Setup(x => x.Headers).Returns(headers.Object);
            response.Setup(x => x.Body).Returns(body.Object);
            sockets.Setup(x => x.IsWebSocketRequest).Returns(false);
            var query = new MyQueryCollection();
            request.Setup(x => x.Query).Returns(query);
            query.Add("doc", "EF2FF98720E34A2EA29E619977A5F04A");
            query.Add("el", "lala");
            query.Add("ev", "lala");
            var handler = new PostEventHandler(null);
            bool result = await handler.ProcessRequest(http.Object);
            Assert.True(result);
        }

        class MyQueryCollection : IQueryCollection
        {
            readonly Dictionary<string, StringValues> _map = new Dictionary<string, StringValues>();

            public StringValues this[string key] => _map[key];

            public int Count => _map.Count;

            public ICollection<string> Keys => _map.Keys;

            public bool ContainsKey(string key) => _map.ContainsKey(key);

            public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
            {
                return _map.GetEnumerator();
            }

            public bool TryGetValue(string key, out StringValues value)
            {
                return _map.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _map.GetEnumerator();
            }

            public void Add(string key, string value)
            {
                _map.Add(key, new StringValues(value));
            }
        }

        [Fact]
        public void ClientEventMessageExtraData()
        {
            var x = new ClientEventMessage
            {
                ExtraData = "lala"
            };
            Assert.Equal("lala", x.ExtraData);
        }

        [Fact]
        public void UseLaraEmpty()
        {
            var app = new Mock<IApplicationBuilder>();
            Assert.Same(app.Object, ApplicationBuilderLaraExtensions.UseLara(app.Object));
        }

        [Fact]
        public void PageContextSocket()
        {
            var http = new Mock<HttpContext>();
            var document = new Document(new MyPage());
            var page = new PageContext(http.Object, null, document);
            var socket = new Mock<WebSocket>();
            page.Socket = socket.Object;
            Assert.Same(socket.Object, page.Socket);
        }

        [Fact]
        public async void CannotFlushAjax()
        {
            var http = new Mock<HttpContext>();
            var document = new Document(new MyPage());
            var page = new PageContext(http.Object, null, document);
            await DomOperationsTesting.ThrowsAsync<InvalidOperationException>(async ()
                => await page.Navigation.FlushPartialChanges());
        }

        [Fact]
        public async void FlushSendsMessage()
        {
            var http = new Mock<HttpContext>();
            var document = new Document(new MyPage());
            var socket = new Mock<WebSocket>();
            var page = new PageContext(http.Object, null, document)
            {
                Socket = socket.Object
            };
            var button = new Button();
            document.OpenEventQueue();
            document.Body.AppendChild(button);
            await page.Navigation.FlushPartialChanges();
            Assert.Empty(document.GetQueue());
            Assert.Same(socket.Object, page.Socket);
        }

        [Fact]
        public void EmptyArraySegment()
        {
            var x = PostEventHandler.BuildArraySegment("");
            Assert.Empty(x.Array);
        }

        [Fact]
        public void ValidateAddress()
        {
            Published.ValidateAddress("test");
            Assert.ThrowsAny<ArgumentException>(() => Published.ValidateAddress(""));
            Assert.ThrowsAny<ArgumentException>(() => Published.ValidateAddress(null));
        }

        [Fact]
        public void ValidateMethod()
        {
            Published.ValidateMethod("test");
            Assert.ThrowsAny<ArgumentException>(() => Published.ValidateMethod(""));
            Assert.ThrowsAny<ArgumentException>(() => Published.ValidateMethod(null));
        }

        [Fact]
        public void UnpublishMethod()
        {
            var x = new Published();
            x.Publish(new WebServiceContent
            {
                Address = "/myws",
                Method = "PUT"
            });
            var combined = Published.CombinePathMethod("/myws", "PUT");
            Assert.True(x.TryGetNode(combined, out _));
            x.UnPublish("/myws", "PUT");
            Assert.False(x.TryGetNode(combined, out _));
        }

        [Fact]
        public void WebServiceSessionNotFound()
        {
            var http = new Mock<HttpContext>();
            var context = new WebServiceContext
            {
                Http = http.Object
            };
            var request = new Mock<HttpRequest>();
            http.Setup(x => x.Request).Returns(request.Object);
            var cookies = new Mock<IRequestCookieCollection>();
            request.Setup(x => x.Cookies).Returns(cookies.Object);
            string temp;
            cookies.Setup(x => x.TryGetValue(GlobalConstants.CookieSessionId, out temp)).Returns(false);
            Assert.False(context.TryGetSession(out _));
        }

        [Fact]
        public void WebServiceCustomCode()
        {
            var x = new WebServiceContext
            {
                StatusCode = HttpStatusCode.BadRequest
            };
            Assert.Equal(HttpStatusCode.BadRequest, x.StatusCode);
        }

        [Fact]
        public async void PostEventHandlerSkipsRequests()
        {
            var http = new Mock<HttpContext>();
            var post = new PostEventHandler(null);
            var request = new Mock<HttpRequest>();
            http.Setup(x => x.Request).Returns(request.Object);
            request.Setup(x => x.Path).Returns(PostEventHandler.EventPrefix);
            request.Setup(x => x.Method).Returns("LALA");
            var websockets = new Mock<WebSocketManager>();
            http.Setup(x => x.WebSockets).Returns(websockets.Object);
            Assert.False(await post.ProcessRequest(http.Object));
        }

        [Fact]
        public async void PostEventHandlerNoElement()
        {
            var http = new Mock<HttpContext>();
            var page = new MyPage();
            var document = new Document(page);
            var context = new PostEventContext
            {
                Document = document,
                Http = http.Object,
                Parameters = new EventParameters
                {
                    ElementId = "aaa"
                }
            };
            var sockets = new Mock<WebSocketManager>();
            http.Setup(x => x.WebSockets).Returns(sockets.Object);
            var response = new Mock<HttpResponse>();
            http.Setup(x => x.Response).Returns(response.Object);
            response.SetupProperty(x => x.StatusCode);
            var headers = new Mock<IHeaderDictionary>();
            response.Setup(x => x.Headers).Returns(headers.Object);
            var body = new Mock<Stream>();
            response.Setup(x => x.Body).Returns(body.Object);
            await PostEventHandler.ProcessRequestDocument(context);
            Assert.Equal(200, response.Object.StatusCode);
        }
    }
}
