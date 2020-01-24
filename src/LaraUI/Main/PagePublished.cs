/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    internal sealed class PagePublished : IPublishedItem
    {
        private readonly Func<IPage> _factory;

        private HttpStatusCode StatusCode { get; } = HttpStatusCode.OK;

        public PagePublished(Func<IPage> factory)
        {
            _factory = factory;
        }

        public PagePublished(Func<IPage> factory, HttpStatusCode status)
            : this(factory)
        {
            StatusCode = status;
        }

        public async Task Run(Application app, HttpContext http, LaraOptions options)
        {
            var connection = GetConnection(app, http);
            var execution = new PageContext(app, http, connection);
            var page = CreateInstance();
            var document = connection.CreateDocument(page, app.KeepAliveInterval);
            execution.DocumentInternal = document;
            if (await RunPage(app, http, page, options).ConfigureAwait(false))
            {
                await ProcessGetResult(http, document, execution, StatusCode);
            }
            if (document.CanDiscard)
            {
                await connection.Discard(document.VirtualId);
            }
        }

        internal static async Task<bool> RunPage(Application app, HttpContext http, IPage page, LaraOptions options)
        {
            try
            {
                await page.OnGet();
                return true;
            }
            catch (StatusCodeException status)
            {
                await ReplyStatusCodeError(app, http, status, options);
                return false;
            }
        }

        internal static async Task ReplyStatusCodeError(Application app, HttpContext http, StatusCodeException status, LaraOptions options)
        {
            if (app.ErrorPages.TryGetPage(status.StatusCode, out var page))
            {
                await page.Run(app, http, options);
            }
            else
            {
                await MiddlewareCommon.SendStatusReply(http, status.StatusCode, status.Message);
            }
        }

        internal IPage CreateInstance() => _factory();

        internal static async Task ProcessGetResult(HttpContext http, Document document, PageContext execution, HttpStatusCode code)
        {
            if (!string.IsNullOrEmpty(execution.RedirectLocation))
            {
                http.Response.Redirect(execution.RedirectLocation);
            }
            else
            {
                document.OpenEventQueue();
                var html = WriteDocument(execution.Document);
                await ReplyDocument(http, html, code);
            }
        }

        internal static Connection GetConnection(Application app, HttpContext http)
        {
            if (MiddlewareCommon.TryFindConnection(app, http, out var connection))
            {
                return connection;
            }
            else
            {
                return CreateConnection(app, http);
            }
        }

        private static Connection CreateConnection(Application app, HttpContext http)
        {
            var connection = app.CreateConnection(http.Connection.RemoteIpAddress);
            http.Response.Cookies.Append(GlobalConstants.CookieSessionId,
                connection.Id.ToString(GlobalConstants.GuidFormat, CultureInfo.InvariantCulture));
            return connection;
        }

        private static string WriteDocument(Document document)
        {
            var writer = new DocumentWriter(document);
            writer.Print();
            return writer.ToString();
        }

        private static async Task ReplyDocument(HttpContext http, string html, HttpStatusCode code)
        {
            MiddlewareCommon.SetStatusCode(http, code);
            MiddlewareCommon.AddHeaderTextHtml(http);
            MiddlewareCommon.AddHeaderPreventCaching(http);
            await MiddlewareCommon.WriteUtf8Buffer(http, html);
        }
    }
}
