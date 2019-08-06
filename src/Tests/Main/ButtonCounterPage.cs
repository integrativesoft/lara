/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;

namespace Integrative.Lara.Tests.Main
{
    class ButtonCounterPage : IPage
    {
        public const string ButtonId = "MyCounterButton";

        readonly bool _useSockets;

        public ButtonCounterPage(bool useSockets)
        {
            _useSockets = useSockets;
        }

        int counter = 0;

        public string LastPath { get; private set; }

        public Task OnGet()
        {
            var document = LaraUI.Page.Document;
            var span = Element.Create("span");
            var text = new TextNode("Click me");
            var button = Element.Create("button", ButtonId);
            button.AppendChild(text);
            document.Body.AppendChild(span);
            document.Body.AppendChild(button);
            button.On(new EventSettings
            {
                EventName = "click",
                LongRunning = _useSockets,
                Handler = () =>
                {
                    counter++;
                    text.Data = $"Clicked {counter} times";
                    LastPath = LaraUI.Context.Http.Request.Path;
                    return Task.CompletedTask;
                }
            });
            return Task.CompletedTask;
        }
    }
}
