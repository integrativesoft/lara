/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tests.Middleware;
using Xunit;

namespace Integrative.Lara.Tests.Delta
{
    public class LocatorTesting : DummyContextTesting
    {
        [Fact]
        public void LocateElementWithId()
        {
            var x = Element.Create("span", "x");
            var locator = NodeLocator.FromNode(x);
            Assert.Equal(x.Id, locator.StartingId);
        }
    }
}
