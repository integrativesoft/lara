/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    class LockingSample
    {
        readonly Button _button;

        public Element Root { get; }

        public LockingSample()
        {
            _button = new Button
            {
                Class = "btn btn-primary my-2"
            };
            Root = Element.Create("div");
            Root.Class = "form-row";
            Root.AppendChild(_button);
            _button.AppendChild(new TextNode("Action that locks UI"));
            _button.On(new EventSettings
            {
                EventName = "click",
                Block = true,
                Handler = () => Task.Delay(1000),
                BlockOptions = new BlockOptions
                {
                    ShowHtmlMessage = GetSpinnerHtml("Brewing coffee..."),
                }
            });
        }

        public static string GetSpinnerHtml(string message)
        {
            var div = Element.Create("div");
            div.Class = "d-flex justify-content-center";
            var builder = new LaraBuilder(div);
            builder.Push("div", "spinner-border")
                .Attribute("role", "status")
            .Pop()
            .Push("div", "ml-2")
                .AddTextNode(message)
            .Pop();
            return div.GetHtml();
        }
    }
}
