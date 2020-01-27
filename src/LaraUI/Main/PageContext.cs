/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    internal sealed class PageContext : BaseContext, IPageContext
    {
        public Document? DocumentInternal { get; internal set; }

        public Document Document
            => DocumentInternal ?? throw new MissingMemberException(nameof(PageContext), nameof(Document));

        private readonly JSBridge _bridge;
        private readonly Navigation _navigation;
        private readonly Connection _connection;

        public PageContext(Application app, HttpContext http, Connection connection)
            : base(app, http)
        {
            _navigation = new Navigation(this);
            _bridge = new JSBridge(this);
            _connection = connection;
        }

        public Session Session => _connection.Session;

        internal WebSocket? Socket { get; set; }

        public IJSBridge JSBridge => _bridge;
        public INavigation Navigation => _navigation;

        public string? RedirectLocation => _navigation.RedirectLocation;

        internal void SetExtraData(string? data) => _bridge.EventData = data;
    }
}
