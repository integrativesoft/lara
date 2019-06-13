/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;

namespace Integrative.Lara.Middleware
{
    class PostEventContext
    {
        public HttpContext Http { get; set; }
        public WebSocket Socket { get; set; }
        public EventParameters Parameters { get; set; }
        public Element Element { get; set; }
    }
}
