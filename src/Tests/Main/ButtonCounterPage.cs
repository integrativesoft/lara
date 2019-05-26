/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.DOM;
using Integrative.Lara.Main;
using System.Threading.Tasks;

namespace Integrative.Lara.Tests.Main
{
    class ButtonCounterPage : IPage
    {
        public const string ButtonId = "MyCounterButton";

        int counter = 0;

        public string LastPath { get; private set; } 

        public Task OnGet(IPageContext context)
        {
            var span = Element.Create("span");
            var text = new TextNode("Click me");
            var button = Element.Create("button", ButtonId);
            button.AppendChild(text);
            context.Document.Body.AppendChild(span);
            context.Document.Body.AppendChild(button);
            button.On("click", app =>
            {
                counter++;
                text.Data = $"Clicked {counter} times";
                LastPath = app.Http.Request.Path;
                return Task.CompletedTask;
            });
            return Task.CompletedTask;
        }
    }
}
