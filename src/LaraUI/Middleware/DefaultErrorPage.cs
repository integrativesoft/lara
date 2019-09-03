/*
Copyright (c) 2019 Integrative Software LLC
Created: 9/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tools;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    class DefaultErrorPage : IPage
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public Task OnGet()
        {
            LoadBootstrap();
            ShowContent();
            return Task.CompletedTask;
        }

        private static void LoadBootstrap()
        {
            var head = LaraUI.Page.Document.Head;
            head.AppendChild(new Link
            {
                Rel = "stylesheet",
                HRef = "https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css"
            });
            head.AppendChild(new Script
            {
                Src = "https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.7/umd/popper.min.js",
                Defer = true
            });
            head.AppendChild(new Script
            {
                Src = "https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/js/bootstrap.min.js",
                Defer = true
            });
        }

        private void ShowContent()
        {
            var builder = new LaraBuilder(LaraUI.Page.Document.Body);
            builder.Push("div", "container")
                .Push("div", "jumbotron")
                    .Push("img")
                        .Attribute("src", ServerLauncher.ErrorAddress + ".svg")
                        .Attribute("height", "100px")
                    .Pop()
                    .Push("h1", "display-4")
                        .AddTextNode(Title)
                    .Pop()
                    .Push("p", "lead")
                        .AddTextNode(Message)
                    .Pop()
                .Pop()
            .Pop();
        }
    }
}
