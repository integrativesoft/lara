/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using Integrative.Lara.DOM;
using Integrative.Lara.Tests.Main;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Integrative.Lara.Tests.Delta
{
    public class LocatorTesting
    {
        [Fact]
        public void LocateElementWithId()
        {
            var x = new Element("span", "x");
            var locator = ElementLocator.FromElement(x);
            Assert.Equal(x.Id, locator.StartingId);
            Assert.NotNull(locator.Steps);
            Assert.Empty(locator.Steps);
        }

        [Fact]
        public void LocateWithSteps()
        {
            var x0 = new Element("div", "x0");
            x0.AppendChild(new Element("div"));
            x0.AppendChild(new Element("div"));
            x0.AppendChild(new Element("div"));
            var a = new Element("div");
            x0.AppendChild(a);
            var b = new Element("div");
            a.AppendChild(b);
            var locator = ElementLocator.FromElement(b);
            var steps = locator.Steps;
            Assert.Equal("x0", locator.StartingId);
            Assert.Equal(2, steps.Count);
            Assert.Equal(0, steps[0]);
            Assert.Equal(3, steps[1]);
        }
    }
}
