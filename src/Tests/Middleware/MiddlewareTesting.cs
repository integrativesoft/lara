/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Integrative.Lara.Middleware;
using Integrative.Lara.Tests.Main;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
            var result = await EventParameters.ReadBody(http.Object);
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
            var context = new PageContext(http.Object, document);
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
            http.Setup(x => x.Request).Returns(request.Object);
            http.Setup(x => x.Response).Returns(response.Object);
            request.Setup(x => x.Method).Returns("POST");
            request.Setup(x => x.Path).Returns("/_event");
            request.Setup(x => x.Cookies).Returns(cookies.Object);
            response.Setup(x => x.Headers).Returns(headers.Object);
            response.Setup(x => x.Body).Returns(body.Object);
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
    }
}
