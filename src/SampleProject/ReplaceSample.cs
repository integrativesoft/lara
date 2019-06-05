/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    class ReplaceSample
    {
        readonly Button _button;

        public ReplaceSample()
        {
            _button = new Button
            {
                Class = "btn btn-primary my-2"
            };
        }

        public Element Build()
        {
            var div = Element.Create("div");
            div.Class = "form-row";
            div.AppendChild(_button);
            _button.AppendChild(new TextNode("Navigate to Google.com"));
            _button.On(new EventSettings
            {
                EventName = "click",
                Handler = ButtonHandler,
            });
            return div;
        }

        private Task ButtonHandler(IPageContext arg)
        {
            arg.Navigation.Replace("https://google.com");
            return Task.CompletedTask;
        }
    }
}
