using Integrative.Lara;
using System;
using System.Threading.Tasks;

namespace Boilerplate
{
    public static class Program
    {
        public static async Task Main()
        {
            // create and start application
            const int port = 8182;
            using var app = new Application();
            app.PublishPage("/", () => new MyCounterComponent { Value = 5 });
            await app.Start(new StartServerOptions { Port = port });

            // print address on console (set project's output type to WinExe to avoid console)
            var address = $"http://localhost:{port}";
            Console.WriteLine($"Listening on {address}/");

            // helper function to launch browser (comment out as needed)
            LaraUI.LaunchBrowser(address);

            // wait for ASP.NET Core shutdown
            await app.WaitForShutdown();
        }
    }

    internal class MyCounterComponent : WebComponent
    {
        private int _value;
        public int Value { get => _value; set => SetProperty(ref _value, value); }

        public MyCounterComponent()
        {
            ShadowRoot.Child(
                new HtmlDivElement()
                    .OnSourceChange(this, x => x.InnerText = Value.ToString()),
                new HtmlButtonElement {  InnerText = "Increase" }
                    .Event("click", () => Value++)
                );
        }
    }
}
