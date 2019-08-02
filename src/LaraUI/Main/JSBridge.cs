/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using System;
using System.Threading.Tasks;

namespace Integrative.Lara.Main
{
    sealed class JSBridge : IJSBridge
    {
        readonly PageContext _parent;

        public string EventData { get; internal set; }

        public JSBridge(PageContext parent)
        {
            _parent = parent;
        }

        public void Submit(string javaScriptCode)
        {
            _parent.Document.Enqueue(new SubmitJsDelta
            {
                Code = javaScriptCode
            });
        }

        public void OnMessage(string key, Func<IPageContext, Task> handler)
        {
            _parent.Document.OnMessage(key, handler);
        }

        public void ServerEventsOn() => _parent.Document.ServerEventsOn();

        public Task ServerEventsOff() => _parent.Document.ServerEventsOff();
    }
}
