/*
Copyright (c) 2019 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;

namespace Integrative.Lara.Main
{
    abstract class BaseContext : ILaraContext
    {
        public HttpContext Http { get; }
        public Application Application { get; }

        internal BaseContext(Application app, HttpContext http)
        {
            LaraUI.InternalContext.Value = this;
            Application = app;
            Http = http;
        }
    }
}
