# Lara Web Engine

Lara is a lightweight, high-performance cross-platform C# library for Web UI development with DOM manipulation and server-side rendering.

The purpose of Lara is to give you full control of the HTML document tree from the server in C#.

- Cross-Platform: .NET Standard and runs on Windows, Linux, and MacOS. Tested on NET Core and NET Framework.
- Web UI: Create HTML5 web applications with full control of the DOM tree in C#.
- Server-side rendering: The server keeps a virtual copy of the page and flushes the modifications to the browser.

Lara can be used to develop either websites or desktop apps with an HTML5 frontend.

The source code contains a [sample project](https://github.com/integrativesoft/lara/tree/master/src/SampleProject).

We welcome questions and feedback! Our Gitter channel is [here](https://gitter.im/laraui) and we also have a general feedback form  [here](https://integrative.b-cdn.net/feedback_lara_briskforms.html).

## Sample application

To create a web page:
- Create a class that implements the interface 'IPage'
- Call LaraUI.Publish(...) to make your page available

Lara uses async/await to allow for high server throughput. If you're new to these, simply follow the examples that return 'Task.CompletedTask' at the end.

Example:

```csharp
namespace SampleProject
{
    class Program
    {
        static async Task Main()
        {
            // Load classes decorated with Lara attributes.
            LaraUI.PublishAssemblies();  // alternatively, use LaraUI.Publish(..) to pick individual classes

            // Start web server.
            var host = await LaraUI.StartServer(new StartServerOptions
            {
                Port = 8181,  // alternatively, leave as 0 to assign dynamic port
                AllowLocalhostOnly = true  // accept connection from current machine only (default)
            });

            // Write address in console.
            string address = LaraUI.GetFirstURL(host);
            Console.WriteLine($"Server listening in {address}.");

            // Launch browser tab. Alternatively, comment out and direct the user to localhost:8181.
            LaraUI.LaunchBrowser(address);
            
            // Wait for termination.
            Console.WriteLine("Press Ctrl+C to terminate");
            await host.WaitForShutdownAsync();
        }
    }

    [LaraPage(Address = "/")]
    class MyPage : IPage
    {
        int counter = 0;

        public Task OnGet()
        {
            var button = Element.Create("button");
            button.AppendText("Click me");
            button.On("click", app =>
            {
                counter++;
                text.Data = $"Clicked {counter} times";
                return Task.CompletedTask;
            });
            LaraUI.Page.Document.Body.AppendChild(button);
            return Task.CompletedTask;
        }
    }
}
```

## Integrating Lara into your existing web server

The example above calls into Lara's StartServer() method to create a default web host.

Alternatively, you can and add Lara as one more service with the following line:

```csharp
app.UseLara(options);
```

## Developing a desktop application

Here you need to choose a tool to load your locally-hosted website inside a desktop window. Some options available:
- Creating an electron app using [electron-cgi](https://github.com/ruidfigueiredo/electron-cgi) (recommended). This seems to be the most resilient way to run a web desktop app. It comes at the cost of having to distribute electron with nodejs.
- Using [Chromely](https://github.com/chromelyapps/Chromely). Currently supports Windows and Linux. Their Windows version based on CefSharp works very well.

## How does Lara work?

Whenever the browser triggers a registered event (e.g. click on a button), it sends to the server a message saying that the button was clicked. The server executes the code associated with the event, manipulating the server's copy of the page, and replies a JSON message with the delta between server and client.

## Feedback

We'd love to hear your feedback. Write to us [here](https://integrative.b-cdn.net/feedback_lara_briskforms.html).
