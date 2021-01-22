/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    internal class PostEventContext
    {
        public Application Application { get; set; }
        public HttpContext Http { get; set; }

        public EventParameters? Parameters { get; set; }
        public WebSocket? Socket { get; set; }
        public Connection? Connection { get; set; }
        public Document? Document { get; set; } 
        public Element? Element { get; set; }

        public PostEventContext(Application app, HttpContext http)
        {
            Application = app;
            Http = http;
        }

        public bool SocketRemainsOpen()
            => Document != null
            && Parameters != null
            && Document.SocketRemainsOpen(Parameters.EventName);

        public bool IsWebSocketRequest =>
            Http.WebSockets.IsWebSocketRequest;

        public virtual Task<TaskCompletionSource<bool>> GetSocketCompletion()
        {
            var socket = Socket ?? throw new MissingMemberException(nameof(PostEventContext), nameof(Socket));
            return GetDocument().GetSocketCompletion(socket);
        }

        public Document GetDocument()
        {
            return Document ?? throw new MissingMemberException(nameof(PostEventContext), nameof(Document));
        }

        public Connection GetConnection()
        {
            return Connection ?? throw new MissingMemberException(nameof(PostEventContext), nameof(Connection));
        }

        public WebSocket GetSocket()
        {
            return Socket ?? throw new MissingMemberException(nameof(PostEventContext), nameof(Socket));
        }

        public EventParameters GetParameters()
        {
            return Parameters ?? throw new MissingMemberException(nameof(PostEventContext), nameof(EventParameters));
        }
    }
}
