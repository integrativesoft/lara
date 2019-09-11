/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using Xunit;

namespace Integrative.Lara.Tests.Delta
{
    public class LocatorTesting
    {
        [Fact]
        public void LocateElementWithId()
        {
            var x = Element.Create("span", "x");
            var locator = ElementLocator.FromElement(x);
            Assert.Equal(x.Id, locator.StartingId);
            Assert.NotNull(locator.Steps);
            Assert.Empty(locator.Steps);
        }

        [Fact]
        public void LocateWithSteps()
        {
            var x0 = Element.Create("div", "x0");
            x0.AppendChild(Element.Create("div"));
            x0.AppendChild(Element.Create("div"));
            x0.AppendChild(Element.Create("div"));
            var a = Element.Create("div");
            x0.AppendChild(a);
            var b = Element.Create("div");
            a.AppendChild(b);
            var locator = ElementLocator.FromElement(b);
            var steps = locator.Steps;
            Assert.Equal(b.ParentElement.Id, locator.StartingId);
            Assert.Single(steps);
            Assert.Equal(0, steps[0]);
        }
    }
}
