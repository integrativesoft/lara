/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Integrative.Lara.Tools;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.Middleware
{
    public class WebServicesTesting
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
            var x = new LaraWebService();
            Assert.Equal("POST", x.Method);
            Assert.Equal("application/json", x.ContentType);
        }

        [Fact]
        public void LaraWebServiceAttributeProperties()
        {
            var x = new LaraWebService
            {
                Address = "/",
                ContentType = "lala",
                Method = "PUT"
            };
            Assert.Equal("/", x.Address);
            Assert.Equal("lala", x.ContentType);
            Assert.Equal("PUT", x.Method);
        }

        [LaraWebService(Address = "/myWS")]
        class MyWebService : IWebService
        {
            public Task<string> Execute()
            {
                return Task.FromResult(string.Empty);
            }
        }

        [LaraPage("/myPage")]
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
            LaraUI.ClearAll();
            LaraUI.PublishAssemblies();

            var combined = Published.CombinePathMethod("/myWS", "POST");
            var found = LaraUI.TryGetNode(combined, out var item);
            Assert.True(found);
            var service = item as WebServicePublished;
            Assert.NotNull(service);
            Assert.NotNull(service.Factory());
        }

        [Fact]
        public void PublishAssembliesPage()
        {
            LaraUI.ClearAll();
            LaraUI.PublishAssemblies();

            bool found = LaraUI.TryGetNode("/myPage", out var item);
            Assert.True(found);
            var page = item as PagePublished;
            Assert.NotNull(page.CreateInstance());
        }

        [Fact]
        public void UnpublishWebservice()
        {
            LaraUI.ClearAll();
            LaraUI.PublishAssemblies();
            LaraUI.UnPublish("/myWS", "POST");
            var combined = Published.CombinePathMethod("/myWS", "POST");
            Assert.False(LaraUI.TryGetNode(combined, out _));
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
    }
}
