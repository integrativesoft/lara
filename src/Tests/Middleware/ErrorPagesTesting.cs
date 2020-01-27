/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 9/2019
Author: Pablo Carbonell
*/

using Xunit;

namespace Integrative.Lara.Tests.Middleware
{
    public class ErrorPagesTesting : DummyContextTesting
    {
        [Fact]
        public void DefaultNotFoundRuns()
        {
            var pages = new ErrorPages(Context.Application.GetPublished());
            var page = pages.GetPage(System.Net.HttpStatusCode.NotFound);
            Assert.NotNull(page);
        }

        [Fact]
        public void DefaultServerErrorRuns()
        {
            var pages = new ErrorPages(Context.Application.GetPublished());
            var found = pages.TryGetPage(System.Net.HttpStatusCode.InternalServerError, out var page);
            Assert.True(found);
            Assert.NotNull(page);
        }

    }
}
