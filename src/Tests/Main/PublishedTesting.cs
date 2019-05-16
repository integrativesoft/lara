/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Xunit;

namespace Integrative.Lara.Tests.Main
{
    public class PublishedTesting
    {
        [Fact]
        public void UnpublishRemoves()
        {
            using (var published = LaraUI.GetPublished())
            {
                published.Publish("/", new StaticContent(new byte[0]));
                published.Publish("/lala", new StaticContent(new byte[0]));
                LaraUI.UnPublish("/");
                Assert.True(published.TryGetNode("/lala", out _));
                Assert.False(published.TryGetNode("/", out _));
            }
        }
    }
}
