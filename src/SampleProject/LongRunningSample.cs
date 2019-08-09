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

        public Element Root { get; }

        public LongRunningSample()
        {
            _button = new Button
            {
                Class = "btn btn-primary my-2"
            };
            _message = Element.Create("div");
            _message.Style = "display: none";
            Root = Element.Create("div");
            Root.Class = "form-row";
            Root.AppendChild(_button);
            Root.AppendChild(_message);
            _button.AppendChild(new TextNode("Long-running event"));
            _button.On(new EventSettings
            {
                EventName = "click",
                LongRunning = true,
                Handler = ButtonHandler,
                Block = true,
                BlockOptions = new BlockOptions
                {
                    ShowElementId = _message.EnsureElementId(),
                }
            });
        }

        private async Task ButtonHandler()
        {
            string[] numbers = { "five", "four", "three", "two", "one" };
            for (int index = 0; index < numbers.Length; index++)
            {
                _message.ClearChildren();
                _message.AppendText(numbers[index]);
                await LaraUI.Page.Navigation.FlushPartialChanges();
                await Task.Delay(700);
            }
        }
    }
}
