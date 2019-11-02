/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using Moq;
using System;

namespace Integrative.Lara.Tests.Middleware
{
    class DummyContext : BaseContext, IDisposable
    {
        private DummyContext(Application app, Mock<HttpContext> http)
            : base(app, http.Object)
        {
            var request = new Mock<HttpRequest>();
            http.Setup(x => x.Request).Returns(request.Object);
            request.Setup(x => x.Path).Returns("/abc");
        }

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
    }
}
