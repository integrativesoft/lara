/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.DOM;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara.Main
{
    sealed class ExecutionContext : IPageContext
    {
        public HttpContext Http { get; }
        public Document Document { get; }

        public ExecutionContext(HttpContext http, Document document)
        {
            Http = http;
            Document = document;
        }
    }
}
