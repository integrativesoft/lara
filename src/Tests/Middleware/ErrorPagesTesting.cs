/*
Copyright (c) 2019 Integrative Software LLC
Created: 9/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Middleware;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Integrative.Lara.Tests.Middleware
{
    public class ErrorPagesTesting
    {
        [Fact]
        public void DefaultNotFoundRuns()
        {
            var pages = new ErrorPages();
            var page = pages.GetPage(System.Net.HttpStatusCode.NotFound);
            Assert.NotNull(page);
        }

        [Fact]
        public void DefaultServerErrorRuns()
        {
            var pages = new ErrorPages();
            var found = pages.TryGetPage(System.Net.HttpStatusCode.InternalServerError, out var page);
            Assert.True(found);
            Assert.NotNull(page);
        }

    }
}
