/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;

namespace Integrative.Lara.Tests.Main
{
    internal class ButtonCounterPage : IPage
    {
        private const string ButtonId = "MyCounterButton";

        private readonly bool _useSockets;

        public ButtonCounterPage(bool useSockets)
        {
            _useSockets = useSockets;
        }

        private int _counter;

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
                    _counter++;
                    text.Data = $"Clicked {_counter} times";
                    return Task.CompletedTask;
                }
            });
            return Task.CompletedTask;
        }
    }
}
