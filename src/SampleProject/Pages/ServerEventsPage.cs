/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;
using Integrative.Lara;

namespace SampleProject.Pages
{
    internal class ServerEventsPage : IPage
    {
        private readonly HtmlSpanElement _span;

        public ServerEventsPage()
        {
            _span = new HtmlSpanElement();
            _span.AppendText("waiting for server event...");
        }

        public Task OnGet()
        {
            LaraUI.Page.JSBridge.ServerEventsOn();
            LaraUI.Page.Document.Body.AppendChild(_span);
            Task.Run(DelayedTask);
            return Task.CompletedTask;
        }

        private async void DelayedTask()
        {
            await Task.Delay(4000);
            using var access = LaraUI.Document.StartServerEvent();
            _span.ClearChildren();
            _span.AppendText("server event executed");
        }
    }
}
