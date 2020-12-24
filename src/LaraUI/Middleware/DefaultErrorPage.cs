/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 9/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;

namespace Integrative.Lara
{
    internal class DefaultErrorPage : IPage
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public Task OnGet()
        {
            LoadBootstrap();
            ShowContent();
            return Task.CompletedTask;
        }

        private static void LoadBootstrap()
        {
            var head = LaraUI.Page.Document.Head;
            head.AppendChild(new HtmlLinkElement
            {
                Rel = "stylesheet",
                HRef = "https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css"
            });
            head.AppendChild(new HtmlScriptElement
            {
                Src = "https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js",
                Defer = true
            });
            head.AppendChild(new HtmlScriptElement
            {
                Src = "https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js",
                Defer = true
            });
        }

        private void ShowContent()
        {
            LaraUI.Document.Body.Children(
                new HtmlDivElement { Class = "container mt-2"} .Children(
                    new HtmlDivElement { Class = "jumbotron" } .Children(
                        new HtmlImageElement
                        {
                            Src = ServerLauncher.ErrorAddress + ".svg",
                            Height = "100px"
                        },
                        Document.CreateElement("h1")
                            .Wrap(x => x.Class = "display-4")
                            .Wrap(x => x.InnerText = Title)
                        ),
                        Document.CreateElement("p")
                            .Wrap(x => x.InnerText = Message)
                    )                    
                );
        }
    }
}
