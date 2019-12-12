/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    [LaraPage(Address = "/server")]
    class ServerEventsPage : IPage
    {
        readonly Button _button;

        public ServerEventsPage()
        {
            _button = new Button()
            {
                Disabled = true
            };
            _button.AppendText("before");
        }

        public Task OnGet()
        {
            LaraUI.Page.JSBridge.ServerEventsOn();
            LaraUI.Page.Document.Body.AppendChild(_button);
            Task.Run(DelayedTask);
            return Task.CompletedTask;
        }

        private async void DelayedTask()
        {
            await Task.Delay(4000);
            using var access = LaraUI.Document.StartServerEvent();
            _button.ClearChildren();
            _button.AppendText("after");
        }
    }
}
