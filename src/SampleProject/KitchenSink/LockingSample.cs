/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;
using System.Web;

namespace SampleProject
{
    class LockingSample
    {
        private const string SpinnerPath = "https://cdnjs.cloudflare.com/ajax/libs/galleriffic/2.0.1/css/loader.gif";

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
                    ShowHtmlMessage = GetSpinnerHtml(" Please wait...")
                }
            });
        }

        public static string GetSpinnerHtml(string message)
        {
            string encoded = HttpUtility.HtmlEncode(" " + message);
            return $"<h1><img src=\"{SpinnerPath}\"/>{encoded}</h1>";
        }
    }
}
