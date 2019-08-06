/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    class LongRunningSample
    {
        readonly Button _button;
        readonly Element _message;

        public LongRunningSample()
        {
            _button = new Button
            {
                Class = "btn btn-primary my-2"
            };
            _message = Element.Create("div");
            _message.Id = "mymessage";
            _message.Style = "display: none";
        }

        public Element Build(Document document)
        {
            var div = Element.Create("div");
            div.Class = "form-row";
            div.AppendChild(_button);
            _button.AppendChild(new TextNode("Long-running event"));
            _button.On(new EventSettings
            {
                EventName = "click",
                LongRunning = true,
                Handler = ButtonHandler,
                Block = true,
                BlockOptions = new BlockOptions
                {
                    ShowElementId = "mymessage",
                }
            });
            document.Body.AppendChild(_message);
            return div;
        }

        private async Task ButtonHandler()
        {
            for (int index = 10; index > 0; index--)
            {
                _message.ClearChildren();
                _message.AppendChild(new TextNode($"Server says to wait {index}"));
                await LaraUI.Page.Navigation.FlushPartialChanges();
                await Task.Delay(1000);
            }
        }
    }
}
