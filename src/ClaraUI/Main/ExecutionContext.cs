/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.DOM;
using Microsoft.AspNetCore.Http;

namespace Integrative.Clara.Main
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
