/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using Integrative.Lara.Main;
using Integrative.Lara.Tools;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    sealed class PostEventHandler : BaseHandler
    {
        public const string EventPrefix = "/_event";
        public const string AjaxMethod = "POST";
        public const int MaxSizeBytes = 1024000;

        public static event EventHandler EventComplete;
        private static readonly EventArgs _eventArgs = new EventArgs();

        public PostEventHandler(RequestDelegate next) : base(next)
        {
        }

        internal override async Task<bool> ProcessRequest(HttpContext http)
        {
            if (http.Request.Path != EventPrefix)
            {
                return false;
            }
            else if (http.WebSockets.IsWebSocketRequest)
            {
                await ProcessWebSocketEvent(http);
                return true;
            }
            else if (http.Request.Method == AjaxMethod)
            {
                await ProcessAjaxRequest(http);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static async Task ProcessWebSocketEvent(HttpContext http)
        {
            var socket = await http.WebSockets.AcceptWebSocketAsync();
            var result = await MiddlewareCommon.ReadWebSocketMessage<EventParameters>(socket, MaxSizeBytes);
            if (result.Item1)
            {
                var context = new PostEventContext
                {
                    Http = http,
                    Socket = socket,
                    Parameters = result.Item2
                };
                await ProcessRequest(context);
            }
            else
            {
                await socket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData,
                    "Bad request", CancellationToken.None);
            }
        }

        private static async Task ProcessAjaxRequest(HttpContext http)
        {
            if (EventParameters.TryParse(http.Request.Query, out var parameters))
            {
                await parameters.ReadMessage(http);
                var post = new PostEventContext
                {
                    Http = http,
                    Parameters = parameters
                };
                await ProcessRequest(post);
            }
            else
            {
                await MiddlewareCommon.SendStatusReply(http, HttpStatusCode.BadRequest, "Bad request");
            }
        }

        private static async Task ProcessRequest(PostEventContext context)
        {
            if (MiddlewareCommon.TryFindConnection(context.Http, out var connection)
                && connection.TryGetDocument(context.Parameters.DocumentId, out var document))
            {
                context.Connection = connection;
                context.Document = document;
                await ProcessRequestDocument(context);
            }
            else
            {
                await SendEvent(context, EventResultType.NoSession);
            }
        }

        internal static async Task ProcessRequestDocument(PostEventContext context)
        {
            Task release;
            var document = context.Document;
            if (document.TryGetElementById(context.Parameters.ElementId, out var element))
            {
                context.Element = element;
                using (var access = await document.Semaphore.UseWaitAsync())
                {
                    release = await RunEvent(context);
                }
                await release;
            }
            else
            {
                await SendEvent(context, EventResultType.NoElement);
            }
        }

        internal static async Task<Task> RunEvent(PostEventContext post)
        {
            var context = new PageContext(post.Http, post.Connection, post.Document)
            {
                Socket = post.Socket
            };
            ProcessMessageIfNeeded(context, post.Parameters);
            return await RunEventHandler(post);
        }

        internal static async Task<Task> RunEventHandler(PostEventContext post)
        {
            if (await MiddlewareCommon.RunHandler(post.Http,
                async () => await post.Element.NotifyEvent(post.Parameters.EventName)))
            {
                string queue = post.Document.FlushQueue();
                return await SendReply(post, queue);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        internal static void ProcessMessageIfNeeded(PageContext context, EventParameters parameters)
        {
            var message = parameters.Message;
            if (message == null)
            {
                return;
            }
            if (message.Values != null)
            {
                ProcessMessage(context.Document, message);
            }
            context.SetExtraData(message.ExtraData);
        }

        private static void ProcessMessage(Document document, ClientEventMessage message)
        {
            document = document ?? throw new ArgumentNullException(nameof(document));
            foreach (var row in message.Values)
            {
                if (document.TryGetElementById(row.ElementId, out var element))
                {
                    element.NotifyValue(row);
                }
            }
        }

        internal static async Task<Task> SendReply(PostEventContext post, string json)
        {
            Task result = Task.CompletedTask;
            if (post.IsWebSocketRequest)
            {
                if (post.SocketRemainsOpen())
                {
                    result = post.GetSocketCompletion();
                }
                else
                {
                    await SendSocketReply(post, json);
                }
            }
            else
            {
                await SendAjaxReply(post.Http, json);
            }
            EventComplete?.Invoke(post.Http, _eventArgs);
            return result;
        }

        private static async Task SendSocketReply(PostEventContext post, string json)
        {
            await FlushMessage(post.Socket, json);
            await CloseSocket(post.Socket);
        }

        public static Task CloseSocket(WebSocket socket)
        {
            return socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        }

        public static async Task FlushMessage(WebSocket socket, string json)
        {
            var buffer = BuildArraySegment(json);
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public static async Task FlushPartialChanges(WebSocket socket, Document document)
        {
            string json = document.FlushQueue();
            await FlushMessage(socket, json);
        }

        internal static ArraySegment<byte> BuildArraySegment(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return new ArraySegment<byte>(new byte[0]);
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                return new ArraySegment<byte>(bytes);
            }
        }

        private static async Task SendAjaxReply(HttpContext http, string json)
        {
            MiddlewareCommon.SetStatusCode(http, HttpStatusCode.OK);
            MiddlewareCommon.AddHeaderJSON(http);
            await MiddlewareCommon.WriteUtf8Buffer(http, json);
        }

        private static async Task SendEvent(PostEventContext post, EventResultType type)
        {
            var reply = new EventResult
            {
                ResultType = type
            };
            string json = reply.ToJSON();
            await SendReply(post, json);
        }
    }
}
