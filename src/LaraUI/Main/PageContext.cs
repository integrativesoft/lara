/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;

namespace Integrative.Lara.Main
{
    sealed class PageContext : IPageContext
    {
        public HttpContext Http { get; }
        public Document Document { get; }

        readonly JSBridge _bridge;
        readonly Navigation _navigation;
        readonly Connection _connection;

        public PageContext(HttpContext http, Connection connection, Document document)
        {
            LaraUI.InternalContext.Value = this;
            Http = http;
            Document = document;
            _navigation = new Navigation(this);
            _bridge = new JSBridge(this);
            _connection = connection;
        }

        public Session Session => _connection.Session;

        internal WebSocket Socket { get; set; }

        public IJSBridge JSBridge => _bridge;
        public INavigation Navigation => _navigation;

        public string RedirectLocation => _navigation.RedirectLocation;

        internal void SetExtraData(string data) => _bridge.EventData = data;
    }
}
