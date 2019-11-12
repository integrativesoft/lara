/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Net;

namespace Integrative.Lara.Tests.Middleware
{
    class DummyContext : BaseContext, IPageContext, IWebServiceContext
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
        }

        public Document Document => throw new NotImplementedException();

        public IJSBridge JSBridge { get; set; }

        public INavigation Navigation => throw new NotImplementedException();

        public Session Session { get; }

        public string RequestBody { get; set; }

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

        public bool TryGetSession(out Session session)
        {
            throw new NotImplementedException();
        }
    }
}
