## About

**Lara Web Engine** is a lightweight, high-performance cross-platform C# library for Web UI development with DOM manipulation and server-side rendering.

>*"[Lara] is similar to server-side Blazor, but is much more lightweight and easier to install. For example, while any type of Blazor requires a whole SDK, Lara is just a NuGet package."* [ScientificProgrammer.net](https://scientificprogrammer.net/2019/08/18/pros-and-cons-of-blazor-for-web-development/?pagename=pros-and-cons-of-blazor)

The purpose of **Lara** is to give you full control of the HTML document tree from the server in C#.

- Cross-Platform: .NET Standard and runs on Windows, Linux, and MacOS. Tested on NET Core and NET Framework.
- Web UI: Create HTML5 web applications with full control of the DOM tree in C#.
- Server-side rendering: The server keeps a virtual copy of the page and flushes the modifications to the browser.

**Lara** can be used to develop either websites or desktop apps with an HTML5 frontend.

The source code contains a [sample project](https://github.com/integrativesoft/lara/tree/master/src/SampleProject). The documentation is available in the [wiki](https://github.com/integrativesoft/lara/wiki).

## Sample application

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
            button.SetInnerText("Click me");
            button.On("click", () =>
            {
                counter++;
                button.SetInnerText($"Clicked {counter} times");
                return Task.CompletedTask;
            });
            LaraUI.Page.Document.Body.AppendChild(button);
            return Task.CompletedTask;
        }
    }
}
```

## Integrating Lara into an existing web server

The example above creates a standalone Lara web server. It's also possible to integrate Lara into an existing ASP.NET Core host. This lets you have some pages, files and webservices with Lara, while keeping other non-Lara services in the same ASP.NET Core host.

To add Lara to an existing ASP.NET Core server, use:

```csharp
public void Configure(IApplicationBuilder app)  
{  
    app.UseLara(new LaraOptions
    {
        // configuration options
    });
} 
```

## Developing a desktop application

Here you need to choose a tool to load your locally-hosted website inside a desktop window. Some options available:

- Creating an electron app using [electron-cgi](https://github.com/ruidfigueiredo/electron-cgi) (recommended). This seems to be the most resilient way to run a web desktop app. It comes at the cost of having to distribute electron with nodejs.
- Using [Chromely](https://github.com/chromelyapps/Chromely). Currently supports Windows and Linux. Their Windows version based on CefSharp works very well.

If you have other tool suggestions that worked out for you, please let me know.

## How does Lara work?

Whenever the browser triggers a registered event (e.g. click on a button), it sends to the server a message saying that the button was clicked. The server executes the code associated with the event, manipulating the server's copy of the page, and replies a JSON message with the delta between server and client.

## Getting started

Lara is available as the NuGet package `Integrative.Lara`. Create a new project, add the NuGet package, and copy and paste the 'Sample standalone application' in this page.

This repository also contains examples. You can run them directly by cloning this repository from the command line:
```
git clone https://github.com/integrativesoft/lara
```

The [wiki](https://github.com/integrativesoft/lara/wiki) contains the documentation for using Lara.

## How to contribute

Did you try out Lara, what was your experience? Did you run into an issue or have any suggestions? How does Lara meet your needs? What functionality would you like to see? Please let me know by either opening an issue or by [direct message](https://www.linkedin.com/in/pablocar/). Thank you!!

**If you like Lara, please give it a star - it helps!**

## Credits

Thanks to [JetBrains](https://www.jetbrains.com/?from=LaraWebEngine) for the license of DotCover.

[![JetBrains](support/jetbrains.svg)](https://www.jetbrains.com/?from=LaraWebEngine)
