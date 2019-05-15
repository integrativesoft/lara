/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.DOM;
using Integrative.Clara.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.IO;
using System.Net;
using Xunit;

namespace Integrative.Clara.Tests.Middleware
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
        public async void ClientReceivesErrorWhenEventCrashes()
        {
            var http = new Mock<HttpContext>();
            var parameters = new Mock<EventParameters>();
            var response = new Mock<HttpResponse>();
            var element = new Mock<Element>("button");
            http.Setup(x => x.Response).Returns(response.Object);
            response.SetupProperty(x => x.StatusCode);
            var headers = new Mock<IHeaderDictionary>();
            response.Setup(x => x.Headers).Returns(headers.Object);
            var body = new Mock<Stream>();
            response.Setup(x => x.Body).Returns(body.Object);
            response.SetupProperty(x => x.ContentLength);
            try
            {
                await PostEventHandler.RunEvent(http.Object, parameters.Object, element.Object);
            }
            catch (ArgumentNullException)
            {
            }
            Assert.Equal(500, response.Object.StatusCode);
        }
    }
}
