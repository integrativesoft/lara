/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    public interface IPageContext
    {
        HttpContext Http { get; }
        Document Document { get; }
        IJSBridge JSBridge { get; }
        INavigation Navigation { get; }
    }

    public interface IJSBridge
    {
        void Submit(string javaScriptCode);
        void OnMessage(string key, Func<IPageContext, Task> handler);
        string EventData { get; }
    }
}
