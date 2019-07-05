/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System.Net;
using Integrative.Lara.Middleware;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara.Main
{
    sealed class WebServiceContext : IWebServiceContext
    {
        public HttpContext Http { get; set; }
        public string RequestBody { get; set; }
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

        public WebServiceContext()
        {
        }

        public bool TryGetSession(out Session session)
        {
            if (MiddlewareCommon.TryFindConnection(Http, out var connection))
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
