/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Integrative.Lara.Tools;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.Middleware
{
    class RemovablePage : IPage
    {
        public Task OnGet() => Task.CompletedTask;
    }

    class RemovableComponent : WebComponent
    {
        public RemovableComponent() : base("x-removable")
        {
        }
    }

    class RemovableService : IWebService
    {
        public Task<string> Execute() => Task.FromResult(string.Empty);
    }

    public class WebServicesTesting : DummyContextTesting
    {
        [Fact]
        public void LaraJsonSerializeType()
        {
            var tool = new LaraJson();
            var data = new MyData
            {
                Counter = 5
            };
            var json = tool.Stringify(data, typeof(MyData));
            var back = tool.Parse<MyData>(json);
            Assert.Equal(data.Counter, back.Counter);
        }

        [DataContract]
        class MyData
        {
            [DataMember]
            public int Counter { get; set; }
        }

        [Fact]
        public void TryParseCatchesSerializationErrors()
        {
            var tool = new LaraJson();
            var ok = tool.TryParse<MyData>("caca", out _);
            Assert.False(ok);
        }

        [Fact]
        public void ParseThrowsBadRequestException()
        {
            var tool = new LaraJson();
            bool found = false;
            try
            {
                tool.Parse<MyData>("caca");
            }
            catch (StatusCodeException e)
            {
                found = true;
                Assert.Equal(HttpStatusCode.BadRequest, e.StatusCode);
            }
            Assert.True(found);
        }

        [Fact]
        public void StatusCodeExceptionDefaultCode()
        {
            var e = new StatusCodeException();
            Assert.Equal(HttpStatusCode.InternalServerError, e.StatusCode);
        }

        [Fact]
        public void StatusCodeExceptionMessageConstructor()
        {
            var e = new StatusCodeException("hello");
            Assert.Equal("hello", e.Message);
        }

        [Fact]
        public void StatusCodeExceptionCodeAndMessage()
        {
            var e = new StatusCodeException(HttpStatusCode.Conflict, "lala");
            Assert.Equal(HttpStatusCode.Conflict, e.StatusCode);
            Assert.Equal("lala", e.Message);
        }

        [Fact]
        public void StatusForbiddenExceptionDefault()
        {
            var e = new StatusForbiddenException();
            Assert.Equal(HttpStatusCode.Forbidden, e.StatusCode);
        }

        [Fact]
        public void StatusForbiddenMessage()
        {
            var e = new StatusForbiddenException("lala");
            Assert.Equal("lala", e.Message);
            Assert.Equal(HttpStatusCode.Forbidden, e.StatusCode);
        }

        [Fact]
        public void StatusForbiddenMessageInner()
        {
            var inner = new InvalidDataContractException();
            var e = new StatusForbiddenException("lala", inner);
            Assert.Equal("lala", e.Message);
            Assert.Equal(HttpStatusCode.Forbidden, e.StatusCode);
            Assert.Same(inner, e.InnerException);
        }

        [Fact]
        public void LaraWebServiceAttributeDefaults()
        {
            var x = new LaraWebServiceAttribute();
            Assert.Equal("POST", x.Method);
            Assert.Equal("application/json", x.ContentType);
        }

        [Fact]
        public void LaraWebServiceAttributeProperties()
        {
            var x = new LaraWebServiceAttribute
            {
                Address = "/",
                ContentType = "lala",
                Method = "PUT"
            };
            Assert.Equal("/", x.Address);
            Assert.Equal("lala", x.ContentType);
            Assert.Equal("PUT", x.Method);
        }

        [LaraWebServiceAttribute(Address = "/myWS")]
        class MyWebService : IWebService
        {
            public Task<string> Execute()
            {
                return Task.FromResult(string.Empty);
            }
        }

        [LaraPageAttribute("/myPage")]
        class MyPage : IPage
        {
            public Task OnGet()
            {
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void PublishAssembliesService()
        {
            const string Address = "/mydummy1";
            using var app = new Application();
            app.PublishService(new WebServiceContent
            {
                Address = Address,
                Method = "POST",
                Factory = () => new DummyWS()
            });
            var combined = Published.CombinePathMethod(Address, "POST");
            var found = app.TryGetNode(combined, out var item);
            Assert.True(found);
            var service = item as WebServicePublished;
            Assert.NotNull(service);
            Assert.NotNull(service.Factory());
        }

        class DummyWS : IWebService
        {
            public Task<string> Execute() => Task.FromResult(string.Empty);
        }

        [Fact]
        public void PublishAssembliesPage()
        {
            const string Address = "/mypapapapa";
            using var app = new Application();
            app.PublishPage(Address, () => new MyPage());
            bool found = app.TryGetNode(Address, out var item);
            Assert.True(found);
            var page = item as PagePublished;
            Assert.NotNull(page.CreateInstance());
        }

        [Fact]
        public void UnpublishWebservice()
        {
            const string Address = "/mylalala";
            using var app = new Application();
            LaraUI.Publish(new WebServiceContent
            {
                Address = Address,
                Factory = () => new MyWebService()
            });
            LaraUI.UnPublish("/mylalala", "POST");
            var combined = Published.CombinePathMethod(Address, "POST");
            Assert.False(app.TryGetNode(combined, out _));
        }

        [Fact]
        public void VerifyTypeException()
        {
            bool found = false;
            try
            {
                AssembliesReader.VerifyType(typeof(MyPage), typeof(Element));
            }
            catch (InvalidOperationException)
            {
                found = true;
            }
            Assert.True(found);
        }

        [Fact]
        public void ClearAllRemovesPublished()
        {
            using var app = new Application();
            app.PublishPage(_pageName, () => new RemovablePage());
            app.PublishService(new WebServiceContent
            {
                Address = _serviceName,
                Factory = () => new RemovableService(),
                Method = "GET"
            });
            var bytes = Encoding.UTF8.GetBytes("hello");
            app.PublishFile(_fileName, new StaticContent(bytes));
            app.PublishComponent(new WebComponentOptions
            {
                ComponentTagName = _componentName,
                ComponentType = typeof(RemovableComponent)
            });
            VerifyFound(app, true);
            app.ClearAllPublished();
            VerifyFound(app, false);
            app.PublishAssemblies();
        }

        readonly string _serviceName = "/removableService" + GetRandom();
        readonly string _componentName = "x-removable" + GetRandom();
        readonly string _pageName = "/removablePage" + GetRandom();
        readonly string _fileName = "/removableFile" + GetRandom();

        static string GetRandom()
        {
            var random = new Random();
            return random.Next(0, int.MaxValue).ToString();
        }

        private void VerifyFound(Application app, bool found)
        {
            Assert.Equal(found, app.TryGetNode(_pageName, out _));
            Assert.Equal(found, app.TryGetNode(_fileName, out _));
            Assert.Equal(found, app.TryGetNode(_serviceName, out _));
            Assert.Equal(found, app.TryGetComponent(_componentName, out _));
        }
    }
}
