/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;
using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using Moq;
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

        [Fact]
        public async void RedirectExecutes()
        {
            var http = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            var request = new Mock<HttpRequest>();
            http.Setup(x => x.Response).Returns(response.Object);
            http.Setup(x => x.Request).Returns(request.Object);
            request.Setup(x => x.Method).Returns("GET");
            var page = new MyRedirectPage();
            var document = new Document(page);
            var context = new PageContext(http.Object, document);
            await page.OnGet(context);
            await PagePublished.ProcessGetResult(http.Object, document, context);
            response.Verify(x => x.Redirect("https://www.google.com"));
        }

        class MyRedirectPage : IPage
        {
            public Task OnGet(IPageContext context)
            {
                context.Navigation.Replace("https://www.google.com");
                return Task.CompletedTask;
            }
        }
    }
}
