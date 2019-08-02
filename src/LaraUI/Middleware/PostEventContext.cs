/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    class PostEventContext
    {
        public HttpContext Http { get; set; }
        public WebSocket Socket { get; set; }
        public EventParameters Parameters { get; set; }
        public Connection Connection { get; set; }
        public Document Document { get; set; }
        public Element Element { get; set; }

        public bool SocketRemainsOpen()
            => Document.SocketRemainsOpen(Parameters.EventName);

        public bool IsWebSocketRequest =>
            Http.WebSockets.IsWebSocketRequest;

        public Task GetSocketCompletion()
            => Document.GetSocketCompletion(Socket);
    }
}
