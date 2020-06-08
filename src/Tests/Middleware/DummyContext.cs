/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Integrative.Lara.Tests.Middleware
{
    internal class DummyContext : BaseContext, IPageContext, IWebServiceContext
    {
        private DummyContext(Application app, Mock<HttpContext> http)
            : base(app, http.Object)
        {
            var request = new Mock<HttpRequest>();
            http.Setup(x => x.Request).Returns(request.Object);
            request.Setup(x => x.Path).Returns("/abc");
            var guid = Connections.CreateCryptographicallySecureGuid();
            var cnx = new Connection(guid, IPAddress.Loopback);
            Session = new Session(cnx);
            JSBridge = _bridge.Object;
        }

        public Document Document => throw new NotImplementedException();

        private readonly Mock<IJSBridge> _bridge = new Mock<IJSBridge>();

        public IJSBridge JSBridge { get; set; }

        public INavigation Navigation => throw new NotImplementedException();

        public Session Session { get; }

        public string RequestBody { get; set; } = string.Empty;

        public HttpStatusCode StatusCode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public static DummyContext Create()
        {
            var app = new Application();
            var http = new Mock<HttpContext>();
            return new DummyContext(app, http);
        }

        public void Dispose()
        {
            Application.Dispose();
        }

        public bool TryGetSession([NotNullWhen(true)] out Session? session)
        {
            throw new NotImplementedException();
        }
    }
}
