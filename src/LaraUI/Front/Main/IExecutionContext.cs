/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.DOM;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    public interface IExecutionContext
    {
        HttpContext Http { get; }
    }

    public interface IPageContext : IExecutionContext
    {
        Document Document { get; }
        IJSBridge JSBridge { get; }
    }

    public interface IJSBridge
    {
        void Submit(string javaScriptCode);
    }
}
