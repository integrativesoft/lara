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
            var document = _parent.Document;
            VerifyQueueOpen(document);
            document.Enqueue(new SubmitJsDelta
            {
                Code = javaScriptCode
            });
        }

        public static void VerifyQueueOpen(Document document)
        {
            if (document == null || !document.QueueOpen)
            {
                throw new InvalidOperationException("This operation is not supported during the page's GET, only in events.");
            }
        }

        public void OnMessage(string key, Func<IPageContext, Task> handler)
        {
            _parent.Document.OnMessage(key, handler);
        }
    }
}
