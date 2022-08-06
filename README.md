## Lara Web Engine

[![License: Apache 2.0](https://img.shields.io/badge/License-Apache--2.0-blue)](https://github.com/integrativesoft/lara/blob/master/LICENSE) [![NuGet version](http://img.shields.io/nuget/v/Integrative.Lara.svg?nocache=1)](https://www.nuget.org/packages/Integrative.Lara/)  [![Download count](https://img.shields.io/nuget/dt/Integrative.Lara.svg)](https://www.nuget.org/packages/Integrative.Lara/)  [![Build Status](https://api.travis-ci.com/integrativesoft/lara.svg?branch=master)](https://travis-ci.org/integrativesoft/lara)  [![Coverage Status](https://coveralls.io/repos/github/integrativesoft/lara/badge.svg?branch=master&lala=3)](https://coveralls.io/github/integrativesoft/lara?branch=master)
==================


**Lara** is a server-side rendering framework for developing **web user interfaces** using C#.

>*"It is similar to server-side Blazor, but is much more lightweight and easier to install. For example, while any type of Blazor requires a whole SDK, Lara is just a NuGet package."* [ScientificProgrammer.net](https://scientificprogrammer.net/2019/08/18/pros-and-cons-of-blazor-for-web-development/?pagename=pros-and-cons-of-blazor)

## Sample application

```csharp
using Integrative.Lara;
using System;
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
            app.PublishPage("/", () => new MyCounterComponent { Value = 5 });
            await app.Start(new StartServerOptions { Port = port });

            // print address on console
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
        private int _value; // triggers PropertyChanged event
        public int Value { get => _value; set => SetProperty(ref _value, value); }

        public MyCounterComponent()
        {
            ShadowRoot.Children = new Node[]
            {
                new HtmlDivElement() // on PropertyChanged, assigns InnerText
                    .Bind(this, x => x.InnerText = Value.ToString()),
                new HtmlButtonElement
                    { InnerText = "Increase" }
                    .Event("click", () => Value++)
            };
        }
    }
}
```

## Adding Lara to an existing web server application

To add Lara to an existing ASP.NET Core server, add to the Startup class or equivalent:

```csharp
private readonly Application _laraApp = new Application();

public void Configure(IApplicationBuilder app)  
{  
    app.UseLara(_laraApp, new LaraOptions
    {
        // configuration options
    });
} 
```

## Creating Desktop applications

To create a desktop container for your web app, here's a few options:

- [electron.js](https://www.electronjs.org/) combined with [electron-cgi](https://github.com/ruidfigueiredo/electron-cgi#readme) library
- [Chromely](https://github.com/chromelyapps/Chromely)
- [neutralinojs](https://github.com/neutralinojs/neutralinojs)

## Getting started

There's no need to download this repository to use Lara, instead, there's a [NuGet package](https://www.nuget.org/packages/Integrative.Lara/).

**Check out the [wiki documentation](https://github.com/integrativesoft/lara/wiki)**

## How does Lara work?

Whenever the browser triggers a registered event (e.g. click on a button), it sends to the server a message saying that the button was clicked. The server executes the code associated with the event, manipulating the server's copy of the page, and replies a JSON message with the delta between server and client.

## How to contribute

**Please send feedback!** Issues, questions, suggestions, requests for features, and success stories. Please let me know by either opening an issue. Thank you!

**If you like Lara, please give it a star - it helps!**

## Credits

Thanks to [JetBrains](https://www.jetbrains.com/?from=LaraWebEngine) for the licenses of Rider and DotCover.

[![JetBrains](support/jetbrains.svg)](https://www.jetbrains.com/?from=LaraWebEngine)
