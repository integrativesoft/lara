/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Integrative.Lara.Middleware;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara.Main
{
    sealed class WebServiceContext : BaseContext, IWebServiceContext
    {
        public string RequestBody { get; set; } = string.Empty;
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public WebServiceContext(Application app, HttpContext http)
            : base(app, http)
        {
        }

        public bool TryGetSession([NotNullWhen(true)] out Session? session)
        {
            if (MiddlewareCommon.TryFindConnection(Application, Http, out var connection))
            {
                session = connection.Session;
                return true;
            }
            else
            {
                session = default;
                return false;
            }
        }
    }
}
