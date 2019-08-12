/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.Components
{
    public class ComponentTesting
    {
        [Fact]
        public void RegisterComponentSucceeds()
        {
            LaraUI.Publish(new WebComponentOptions
            {
                ComponentTagName = "x-com",
                ComponentType = typeof(MyComponent)
            });
            Assert.True(LaraUI.TryGetComponent("x-com", out var type));
            LaraUI.UnPublishWebComponent("x-com");
            Assert.Equal(typeof(MyComponent), type);
            Assert.False(LaraUI.TryGetComponent("x-com", out _));
        }

        class MyComponent : WebComponent
        {
            public MyComponent() : base("x-com")
            {
            }
        }

        class MyPage : IPage
        {
            public Task OnGet()
            {
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void ServerEventsOnSucceeds()
        {
            var context = CreateMockPage();
            Assert.Equal(ServerEventsStatus.Disabled, context.Document.ServerEventsStatus);
            context.JSBridge.ServerEventsOn();
            Assert.Equal(ServerEventsStatus.Connecting, context.Document.ServerEventsStatus);
            context.JSBridge.ServerEventsOff();
            Assert.Equal(ServerEventsStatus.Disabled, context.Document.ServerEventsStatus);
        }

        private PageContext CreateMockPage()
        {
            var http = new Mock<HttpContext>();
            var guid = Guid.Parse("{5166FB58-FB45-4622-90E6-195E2448F2C9}");
            var connection = new Connection(guid, IPAddress.Loopback);
            var page = new MyPage();
            var document = new Document(page);
            return new PageContext(http.Object, connection, document);
        }

    }
}
