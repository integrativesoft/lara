using Integrative.Lara;
using System;
using System.Threading.Tasks;

namespace SampleProject
{
    class Program
    {
        static async Task Main()
        {
            using var app = new Application();
            await app.Start(new StartServerOptions
            {
                // assign fixed port (zero for dynamic assignment)
                Port = 8182,

                // looks for classes decorated with 'Lara' attributes
                PublishAssembliesOnStart = true
            });
            Console.WriteLine("Listening on http://localhost:8182/");
            await app.WaitForShutdown();
        }
    }

    [LaraPage(Address = "/")]
    class MyPage : IPage
    {
        int counter = 0;

        public Task OnGet()
        {
            var button = Element.Create("button");
            button.InnerText = "Click me";
            button.On("click", () =>
            {
                counter++;
                button.InnerText = $"Clicked {counter} times";
                return Task.CompletedTask;
            });
            LaraUI.Page.Document.Body.AppendChild(button);
            return Task.CompletedTask;
        }
    }
}