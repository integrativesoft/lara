/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    internal sealed class PostEventHandler : BaseHandler
    {
        public const string EventPrefix = "/_event";
        public const string AjaxMethod = "POST";
        public const int MaxSizeBytes = 1024000;

        public static event EventHandler? EventComplete;
        private static readonly EventArgs _eventArgs = new EventArgs();

        private readonly Application _app;

        public PostEventHandler(Application app, RequestDelegate next) : base(next)
        {
            _app = app;
        }

        internal override async Task<bool> ProcessRequest(HttpContext http)
        {
            if (http.Request.Path != EventPrefix)
            {
                return false;
            }
            else if (http.WebSockets.IsWebSocketRequest)
            {
                await ProcessWebSocketEvent(_app, http);
                return true;
            }
            else if (http.Request.Method == AjaxMethod)
            {
                await ProcessAjaxRequest(_app, http);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static async Task ProcessWebSocketEvent(Application app, HttpContext http)
        {
            var socket = await http.WebSockets.AcceptWebSocketAsync();
            var result = await MiddlewareCommon.ReadWebSocketMessage<SocketEventParameters>(socket, MaxSizeBytes);
            var context = new PostEventContext(app, http)
            {
                Http = http,
                Socket = socket,
                Parameters = result.Item2
            };
            if (result.Item1)
            {
                await ProcessRequest(context);
            }
            else
            {
                await context.Socket.CloseAsync(WebSocketCloseStatus.InvalidPayloadData,
                    "Bad request", CancellationToken.None);
            }
        }

        internal static async Task ProcessAjaxRequest(Application app, HttpContext http)
        {
            if (EventParameters.TryParse(http.Request.Query, out var parameters))
            {
                await parameters.ReadAjaxMessage(http);
                var post = new PostEventContext(app, http)
                {
                    Parameters = parameters
                };
                await ProcessRequest(post);
            }
            else
            {
                await MiddlewareCommon.SendStatusReply(http, HttpStatusCode.BadRequest, Resources.BadRequest);
            }
        }

        private static async Task ProcessRequest(PostEventContext context)
        {
            if (MiddlewareCommon.TryFindConnection(context.Application, context.Http, out var connection)
                && context.Parameters != null
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
            var document = context.GetDocument();
            var parameters = context.GetParameters();
            var proceed = await document.WaitForTurn(parameters.EventNumber);
            if (!proceed)
            {
                await SendEvent(context, EventResultType.OutOfSequence);
            }
            else if (string.IsNullOrEmpty(parameters.ElementId))
            {
                await ProcessRequestDocument(context, document);
            }
            else if (document.TryGetElementById(parameters.ElementId, out var element))
            {
                context.Element = element;
                await ProcessRequestDocument(context, document);
            }
            else
            {
                await SendEvent(context, EventResultType.NoElement);
            }
        }

        private static async Task ProcessRequestDocument(PostEventContext context, Document document)
        {
            Task release;
            using (await document.Semaphore.UseWaitAsync())
            {
                release = await RunEvent(context);
            }
            await release;
        }

        private static async Task<Task> RunEvent(PostEventContext post)
        {
            var connection = post.GetConnection();
            var document = post.GetDocument();
            var context = new PageContext(post.Application, post.Http, connection)
            {
                Socket = post.Socket,
                DocumentInternal = document
            };
            ProcessMessageIfNeeded(context, post.Parameters);
            return await RunEventHandler(post);
        }

        internal static async Task<Task> RunEventHandler(PostEventContext post)
        {
            if (await MiddlewareCommon.RunHandler(post.Http,
                () => NotifyEventHandler(post)))
            {
                var document = post.GetDocument();
                var queue = document.FlushQueue();
                return await SendReply(post, queue);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        private static Task NotifyEventHandler(PostEventContext post)
        {
            var parameters = post.GetParameters();
            if (post.Element == null)
            {
                var document = post.GetDocument();
                return document.NotifyEvent(parameters.EventName);
            }
            else
            {
                return post.Element.NotifyEvent(parameters.EventName);
            }
        }

        internal static void ProcessMessageIfNeeded(PageContext context, EventParameters? parameters)
        {
            if (parameters == null)
            {
                return;
            }
            var message = parameters.Message;
            if (message != null)
            {
                context.SetExtraData(message.ExtraData);
                if (message.Values != null)
                {
                    ProcessMessage(context.Document, message);
                }
            }
            var files = parameters.Files;
            if (files != null)
            {
                ProcessFiles(context.Document, files);
            }
        }

        private static void ProcessMessage(Document document, ClientEventMessage message)
        {
            document = document ?? throw new ArgumentNullException(nameof(document));
            message = message ?? throw new ArgumentNullException(nameof(message));
            if (message.Values != null)
            {
                foreach (var row in message.Values)
                {
                    if (document.TryGetElementById(row.ElementId, out var element))
                    {
                        element.NotifyValue(row);
                    }
                }
            }
        }

        private static void ProcessFiles(Document document, IFormFileCollection files)
        {
            foreach (var file in files)
            {
                ProcessFile(document, file);
            }
        }

        private static void ProcessFile(Document document, IFormFile file)
        {
            var name = file.Name;
            if (TryParsePrefix(name, GlobalConstants.FilePrefix, out var id))
            {
                if (document.TryGetElementById(id, out var element)
                    && element is InputElement input)
                {
                    input.AddFile(file);
                }
            }
        }

        private static bool TryParsePrefix(string name, string prefix, [NotNullWhen(true)] out string? elementId)
        {
            if (name.StartsWith(prefix, StringComparison.InvariantCulture))
            {
                elementId = name.Substring(prefix.Length);
                return true;
            }
            else
            {
                elementId = default;
                return false;
            }
        }

        internal static async Task<Task> SendReply(PostEventContext post, string json)
        {
            var result = Task.CompletedTask;
            if (post.IsWebSocketRequest)
            {
                if (post.SocketRemainsOpen())
                {
                    var completion = await post.GetSocketCompletion();
                    return completion.Task;
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
            var socket = post.GetSocket();
            await FlushMessage(socket, json);
            await CloseSocket(socket);
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
            var json = document.FlushQueue();
            await FlushMessage(socket, json);
        }

        internal static ArraySegment<byte> BuildArraySegment(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return new ArraySegment<byte>(Array.Empty<byte>());
            }
            else
            {
                var bytes = Encoding.UTF8.GetBytes(json);
                return new ArraySegment<byte>(bytes);
            }
        }

        private static async Task SendAjaxReply(HttpContext http, string json)
        {
            MiddlewareCommon.AddHeaderJSON(http);
            await MiddlewareCommon.WriteUtf8Buffer(http, json);
        }

        private static async Task SendEvent(PostEventContext post, EventResultType type)
        {
            var reply = new EventResult
            {
                ResultType = type
            };
            var json = reply.ToJSON();
            await SendReply(post, json);
        }
    }
}
