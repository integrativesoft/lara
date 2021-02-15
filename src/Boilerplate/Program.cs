using Integrative.Lara;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SampleApp
{
    public static class Program
    {
        public static async Task Main()
        {
            // create and start application
            const int port = 8182;
            using var app = new Application();
            app.PublishPage("/", () => new MyCounterComponent());
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
        private readonly ObservableCollection<string> _names = new ObservableCollection<string>();

        public MyCounterComponent()
        {
            ShadowRoot.Children = new Node[]
            {
                Fragment.ForEach(_names, (string name) => new HtmlDivElement { InnerText = name }),
            };
            _names.Add("Sarah");
            _names.Add("John");
        }
    }
}
