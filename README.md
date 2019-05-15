# Clara

** UNDER DEVELOPMENT, DO NOT USE IN PRODUCTION **

Clara is a lightweight cross-platform C# library for Web UI development with DOM manipulation and server-side rendering.

- Cross-Platform: .NET Standard and runs on Windows, Linux, and MacOS.
- Web UI: Create HTML5 web applications with full control of the DOM tree.
- Server-side rendering: The server keeps a virtual copy of the page and sends to the client the changes after each event.

Clara can be used to either develop websites or desktop apps with an HTML5 frontend.

## Sample application

To create a web page:
- Create a class that derives 'BasePage' and implements the 'OnGet' method
- Call ClaraUI.Publish(...) to make your page available

Source code example:

```csharp
using Integrative.Clara.DOM;
using Integrative.Clara.Main;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;

namespace SampleProject
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Starting...");

            // create home page
            ClaraUI.Publish("/", () => new MyPage());

            // start web server
            var host = await ClaraUI.StartServer();

            // write address in console
            string address = ClaraUI.GetFirstURL(host);
            Console.WriteLine($"Server listening in {address}.");

            // launch browser tab
            ClaraUI.LaunchBrowser(address);

            // wait for termination
            Console.WriteLine("Press Ctrl+C to terminate");
            await host.WaitForShutdownAsync();
        }
    }

    class MyPage : BasePage
    {
        int counter = 0;

        public override Task OnGet(IPageContext context)
        {
            var button = new Element("button");
            var text = new TextNode { Data = "Click me" };
            button.AppendChild(text);
            button.On("click", app =>
            {
                counter++;
                text.Data = $"Clicked {counter} times";
                return Task.CompletedTask;
            });
            context.Document.Body.AppendChild(button);
            return Task.CompletedTask;
        }
    }
}
```

## Integrating Clara into your existing web server

The example above calls into Clara's StartServer() method to create a default web host.

Alternatively, you can and add Clara as one more service with the following line:

```csharp
app.UseMiddleware<ClaraMiddleware>()
```

## Developing a desktop application

Here you need to choose a tool to load your locally-hosted website inside a desktop window. Some options available:
- Creating an electron app using [electron-cgi](https://github.com/ruidfigueiredo/electron-cgi) (recommended). This seems to be the most resilient way to run a web desktop app. It comes at the cost of having to distribute electron with nodejs.
- Using [Chromely](https://github.com/chromelyapps/Chromely). Currently supports Windows and Linux. Some platform-specific troubleshooting is required. Their Windows version based on CefSharp works very well.
- Creating your own desktop window using [GeckoFX](https://www.nuget.org/profiles/geckofx). .NET Core does not seem to be supported, and html5test.com scores lower than using Gecko on Firefox.

## How does Clara work?

Whenever the browser triggers an event (e.g. click on a button), it sends to the server a message saying that the button was clicked. The server executes the code associated with the event, manipulating the server's copy of the page, and replies a JSON message with the delta between server and client.

