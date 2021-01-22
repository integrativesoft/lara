/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    internal sealed class WebServiceContext : BaseContext, IWebServiceContext
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

            session = default;
            return false;
        }
    }
}
