/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tests.DOM;
using Integrative.Lara.Tests.Main;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Integrative.Lara.Tests.Middleware
{
    internal class MyStatusPage : IPage
    {
        public Task OnGet()
        {
            throw new StatusCodeException(HttpStatusCode.Unauthorized);
        }
    }

    internal class MyCustomErrorPage : IPage
    {
        public const string Text = "Custom error page";

        public int Counter { get; private set; }

        public Task OnGet()
        {
            LaraUI.Document.Body.AppendText(Text);
            Counter++;
            return Task.CompletedTask;
        }
    }

    public class MiddlewareTesting : DummyContextTesting
    {
        [Fact]
        public void TryParseMissingDocFails()
        {
            StringValues values;
            var mock = new Mock<IQueryCollection>();
            mock.Setup(x => x.TryGetValue("doc", out values)).Returns(false);
            Assert.False(DiscardParameters.TryParse(mock.Object, out _));
            Assert.False(EventParameters.TryParse(mock.Object, out _));
        }

        [Fact]
        public async void LocalhostFilterTesting()
        {
            var logger = new Mock<ILogger<LocalhostFilter>>();
            var next = new Mock<RequestDelegate>();
            var filter = new LocalhostFilter(next.Object, logger.Object);

            var connectionInfo = new Mock<ConnectionInfo>();
            connectionInfo.Setup(x => x.RemoteIpAddress).Returns(IPAddress.Parse("172.217.4.206"));
            var http = new Mock<HttpContext>();
            http.Setup(x => x.Connection).Returns(connectionInfo.Object);
            var response = new Mock<HttpResponse>();
            http.Setup(x => x.Response).Returns(response.Object);
            response.SetupProperty(x => x.StatusCode);
            var headers = new Mock<IHeaderDictionary>();
            response.Setup(x => x.Headers).Returns(headers.Object);
            var body = new Mock<Stream>();
            response.Setup(x => x.Body).Returns(body.Object);
            response.SetupProperty(x => x.ContentLength);

            await filter.Invoke(http.Object);

            Assert.Equal(403, response.Object.StatusCode);
        }

        [Fact]
        public async void NullBodyReturnsEmptyString()
        {
            var http = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            http.Setup(x => x.Request).Returns(request.Object);
            var result = await MiddlewareCommon.ReadBody(http.Object);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void AddHeaderNeverExpires()
        {
            var http = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            http.Setup(x => x.Response).Returns(response.Object);
            var headers = new Mock<IHeaderDictionary>();
            response.Setup(x => x.Headers).Returns(headers.Object);
            MiddlewareCommon.AddHeaderNeverExpires(http.Object);
            headers.Verify(x => x.Add("Cache-Control", "max-age=31556926"), Times.Exactly(1));
        }

        [Fact]
        public void ProcessMessageSkipsEmptyMessage()
        {
            var http = new Mock<HttpContext>();
            var cx = new Connection(Guid.NewGuid(), IPAddress.Loopback);
            var context = new PageContext(Context.Application, http.Object, cx);
            var parameters = new EventParameters();
            PostEventHandler.ProcessMessageIfNeeded(context, parameters);
        }

        [Fact]
        public async void ConnectionNotFoundSendsReload()
        {
            var http = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var cookies = new Mock<IRequestCookieCollection>();
            var response = new Mock<HttpResponse>();
            var headers = new Mock<IHeaderDictionary>();
            var body = new Mock<Stream>();
            var sockets = new Mock<WebSocketManager>();
            http.Setup(x => x.Request).Returns(request.Object);
            http.Setup(x => x.Response).Returns(response.Object);
            http.Setup(x => x.WebSockets).Returns(sockets.Object);
            request.Setup(x => x.Method).Returns("POST");
            request.Setup(x => x.Path).Returns("/_event");
            request.Setup(x => x.Cookies).Returns(cookies.Object);
            response.Setup(x => x.Headers).Returns(headers.Object);
            response.Setup(x => x.Body).Returns(body.Object);
            sockets.Setup(x => x.IsWebSocketRequest).Returns(false);
            var query = new MyQueryCollection();
            request.Setup(x => x.Query).Returns(query);
            query.Add("doc", "EF2FF98720E34A2EA29E619977A5F04A");
            query.Add("el", "lala");
            query.Add("ev", "lala");
            var next = new Mock<RequestDelegate>();
            var handler = new PostEventHandler(Context.Application, next.Object);
            var result = await handler.ProcessRequest(http.Object);
            Assert.True(result);
        }

        private class MyQueryCollection : IQueryCollection
        {
            private readonly Dictionary<string, StringValues> _map = new Dictionary<string, StringValues>();

            public StringValues this[string key] => _map[key];

            public int Count => _map.Count;

            public ICollection<string> Keys => _map.Keys;

            public bool ContainsKey(string key) => _map.ContainsKey(key);

            public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
            {
                return _map.GetEnumerator();
            }

            public bool TryGetValue(string key, out StringValues value)
            {
                return _map.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return _map.GetEnumerator();
            }

            public void Add(string key, string value)
            {
                _map.Add(key, new StringValues(value));
            }
        }

        [Fact]
        public void ClientEventMessageExtraData()
        {
            var x = new ClientEventMessage
            {
                ExtraData = "lala"
            };
            Assert.Equal("lala", x.ExtraData);
        }

        [Fact]
        public void UseLaraEmpty()
        {
            var app = new Mock<IApplicationBuilder>();
            Assert.Same(app.Object, app.Object.UseLara());
        }

        [Fact]
        public void PageContextSocket()
        {
            var http = new Mock<HttpContext>();
            var cx = new Connection(Guid.NewGuid(), IPAddress.Loopback);
            var page = new PageContext(Context.Application, http.Object, cx);
            var socket = new Mock<WebSocket>();
            page.Socket = socket.Object;
            Assert.Same(socket.Object, page.Socket);
        }

        [Fact]
        public async void CannotFlushAjax()
        {
            var http = new Mock<HttpContext>();
            var cx = new Connection(Guid.NewGuid(), IPAddress.Loopback);
            var page = new PageContext(Context.Application, http.Object, cx);
            await DomOperationsTesting.ThrowsAsync<InvalidOperationException>(async ()
                => await page.Navigation.FlushPartialChanges());
        }

        [Fact]
        public async void FlushSendsMessage()
        {
            var http = new Mock<HttpContext>();
            var document = new Document(new MyPage(), BaseModeController.DefaultKeepAliveInterval);
            var socket = new Mock<WebSocket>();
            var cx = new Connection(Guid.NewGuid(), IPAddress.Loopback);
            var page = new PageContext(Context.Application, http.Object, cx)
            {
                Socket = socket.Object,
                DocumentInternal = document
            };
            var button = new Button();
            document.OpenEventQueue();
            document.Body.AppendChild(button);
            await page.Navigation.FlushPartialChanges();
            Assert.Empty(document.GetQueue());
            Assert.Same(socket.Object, page.Socket);
        }

        [Fact]
        public void EmptyArraySegment()
        {
            var x = PostEventHandler.BuildArraySegment("");
            Assert.NotNull(x.Array);
            Assert.Empty(x.Array);
        }

        [Fact]
        public void ValidateAddress()
        {
            Published.ValidateAddress("test");
            Assert.ThrowsAny<ArgumentException>(() => Published.ValidateAddress(""));
            Assert.ThrowsAny<ArgumentException>(() => Published.ValidateAddress(null));
        }

        [Fact]
        public void ValidateMethod()
        {
            Published.ValidateMethod("test");
            Assert.ThrowsAny<ArgumentException>(() => Published.ValidateMethod(""));
            Assert.ThrowsAny<ArgumentException>(() => Published.ValidateMethod(null));
        }

        [Fact]
        public void UnpublishMethod()
        {
            var service = new Mock<IWebService>();
            using var x = new Published();
            x.Publish(new WebServiceContent
            {
                Address = "/myws",
                Method = "PUT",
                Factory = () => service.Object
            });
            var combined = Published.CombinePathMethod("/myws", "PUT");
            Assert.True(x.TryGetNode(combined, out _));
            x.UnPublish("/myws", "PUT");
            Assert.False(x.TryGetNode(combined, out _));
        }

        [Fact]
        public void WebServiceSessionNotFound()
        {
            var http = new Mock<HttpContext>();
            var context = new WebServiceContext(Context.Application, http.Object);
            var request = new Mock<HttpRequest>();
            http.Setup(x => x.Request).Returns(request.Object);
            var cookies = new Mock<IRequestCookieCollection>();
            request.Setup(x => x.Cookies).Returns(cookies.Object);
            string temp;
            cookies.Setup(x => x.TryGetValue(GlobalConstants.CookieSessionId, out temp)).Returns(false);
            Assert.False(context.TryGetSession(out _));
        }

        [Fact]
        public void WebServiceCustomCode()
        {
            var http = new Mock<HttpContext>();
            var x = new WebServiceContext(Context.Application, http.Object)
            {
                StatusCode = HttpStatusCode.BadRequest
            };
            Assert.Equal(HttpStatusCode.BadRequest, x.StatusCode);
        }

        [Fact]
        public async void PostEventHandlerSkipsRequests()
        {
            var http = new Mock<HttpContext>();
            var next = new Mock<RequestDelegate>();
            var post = new PostEventHandler(Context.Application, next.Object);
            var request = new Mock<HttpRequest>();
            http.Setup(x => x.Request).Returns(request.Object);
            request.Setup(x => x.Path).Returns(PostEventHandler.EventPrefix);
            request.Setup(x => x.Method).Returns("LALA");
            var websockets = new Mock<WebSocketManager>();
            http.Setup(x => x.WebSockets).Returns(websockets.Object);
            Assert.False(await post.ProcessRequest(http.Object));
        }

        [Fact]
        public async void PostEventHandlerNoElement()
        {
            var http = new Mock<HttpContext>();
            var page = new MyPage();
            var document = new Document(page, BaseModeController.DefaultKeepAliveInterval);
            var context = new PostEventContext(Context.Application, http.Object)
            {
                Document = document,
                Parameters = new EventParameters
                {
                    ElementId = "aaa"
                }
            };
            var sockets = new Mock<WebSocketManager>();
            http.Setup(x => x.WebSockets).Returns(sockets.Object);
            var response = new Mock<HttpResponse>();
            http.Setup(x => x.Response).Returns(response.Object);
            response.SetupProperty(x => x.StatusCode);
            var headers = new Mock<IHeaderDictionary>();
            response.Setup(x => x.Headers).Returns(headers.Object);
            var body = new Mock<Stream>();
            response.Setup(x => x.Body).Returns(body.Object);
            await PostEventHandler.ProcessRequestDocument(context);
            var code = response.Object.StatusCode;
            Assert.True(code == 200 || code == 0);
        }

        [Fact]
        public async void SendReplyLeavesSocketOpen()
        {
            var page = new MyPage();
            var document = new Document(page, BaseModeController.DefaultKeepAliveInterval);
            document.ServerEventsOn();
            var post = new Mock<PostEventContext>(Context.Application, Context.Http);
            post.Object.Document = document;
            var parameters = new EventParameters
            {
                EventName = GlobalConstants.ServerSideEvent,
            };
            post.Object.Parameters = parameters;
            var http = new Mock<HttpContext>();
            var ws = new Mock<WebSocketManager>();
            http.Setup(x => x.WebSockets).Returns(ws.Object);
            ws.Setup(x => x.IsWebSocketRequest).Returns(true);
            post.Object.Http = http.Object;
            post.Setup(x => x.GetSocketCompletion()).Returns(CompletionResult);
            await PostEventHandler.SendReply(post.Object, "lala");
            post.Verify(x => x.GetSocketCompletion());
        }

        private static Task<TaskCompletionSource<bool>> CompletionResult()
        {
            var mock = new Mock<TaskCompletionSource<bool>>();
            return Task.FromResult(mock.Object);
        }

        [Fact]
        public async void StatusCodeExceptionProcessed()
        {
            var element = Element.Create("div");
            element.On("click", () => throw new StatusCodeException(HttpStatusCode.Forbidden));
            var http = new Mock<HttpContext>();
            var post = new PostEventContext(Context.Application, http.Object)
            {
                Element = element,
                Parameters = new EventParameters
                {
                    EventName = "click"
                }
            };
            var response = new Mock<HttpResponse>();
            response.SetupProperty(x => x.StatusCode);
            http.Setup(x => x.Response).Returns(response.Object);
            post.Http = http.Object;
            var headers = new Mock<IHeaderDictionary>();
            response.Setup(x => x.Headers).Returns(headers.Object);
            var body = new Mock<Stream>();
            response.Setup(x => x.Body).Returns(body.Object);
            await PostEventHandler.RunEventHandler(post);
            Assert.Equal((int)HttpStatusCode.Forbidden, response.Object.StatusCode);
        }

        [Fact]
        public void ProcessSocketMessageWrongType()
        {
            var ms = new Mock<MemoryStream>();
            var sr = new WebSocketReceiveResult(1, WebSocketMessageType.Close, true);
            var result = MiddlewareCommon.ProcessWebSocketMessage<Element>(100, ms.Object, sr);
            Assert.False(result.Item1);
        }

        [Fact]
        public void ProcessSocketMessageWrongCount()
        {
            var ms = new Mock<MemoryStream>();
            var sr = new WebSocketReceiveResult(101, WebSocketMessageType.Text, true);
            var result = MiddlewareCommon.ProcessWebSocketMessage<Element>(100, ms.Object, sr);
            Assert.False(result.Item1);
        }

        [Fact]
        public void ProcessSocketMessageFailDeserialize()
        {
            var bytes = Encoding.UTF8.GetBytes("hello");
            using var ms = new MemoryStream(bytes);
            var sr = new WebSocketReceiveResult(1, WebSocketMessageType.Text, true);
            var result = MiddlewareCommon.ProcessWebSocketMessage<Element>(100, ms, sr);
            Assert.False(result.Item1);
        }

        [Fact]
        public async void ProcessAjaxBadParameters()
        {
            var http = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var response = new Mock<HttpResponse>();
            var query = new Mock<IQueryCollection>();
            var headers = new Mock<IHeaderDictionary>();
            http.Setup(x => x.Request).Returns(request.Object);
            request.Setup(x => x.Query).Returns(query.Object);
            StringValues values;
            query.Setup(x => x.TryGetValue("doc", out values)).Returns(false);
            response.SetupProperty(x => x.StatusCode);
            http.Setup(x => x.Response).Returns(response.Object);
            response.Setup(x => x.Headers).Returns(headers.Object);
            var body = new Mock<Stream>();
            response.Setup(x => x.Body).Returns(body.Object);
            await PostEventHandler.ProcessAjaxRequest(Context.Application, http.Object);
        }

        [Fact]
        public void PostGetCompletionRuns()
        {
            var page = new MyPage();
            var document = new Mock<Document>(page, 200);
            var socket = new Mock<WebSocket>();
            var post = new PostEventContext(Context.Application, Context.Http)
            {
                Document = document.Object,
                Socket = socket.Object
            };
            var task = post.GetSocketCompletion();
            Assert.NotNull(task);
            document.Verify(x => x.GetSocketCompletion(socket.Object));
        }

        [Fact]
        public void ServerEventCases()
        {
            var socket = new Mock<WebSocket>();
            var status = ServerEventsController.CalculateServerEventsStatus(false, socket.Object);
            Assert.Equal(ServerEventsStatus.Disabled, status);
            status = ServerEventsController.CalculateServerEventsStatus(true, null);
            Assert.Equal(ServerEventsStatus.Connecting, status);
            status = ServerEventsController.CalculateServerEventsStatus(true, socket.Object);
            Assert.Equal(ServerEventsStatus.Enabled, status);
        }

        [Fact]
        public void SessionCloseIgnoresErrors()
        {
            using var connections = new Connections();
            var connection = connections.CreateConnection(IPAddress.Loopback);
            var session = new Session(connection);
            session.Closing += Session_Closing;
            session.Close();
        }

        private static void Session_Closing(object? sender, EventArgs e)
        {
            throw new InvalidOperationException();
        }

        [Fact]
        public void LaraPageDefaultConstructor()
        {
            var page = new LaraPageAttribute();
            Assert.True(string.IsNullOrEmpty(page.Address));
        }

        [Fact]
        public void ServerLauncherUseDeveloperPage()
        {
            var app = new Mock<IApplicationBuilder>();
            ServerLauncher.ConfigureExceptions(app.Object, new StartServerOptions
            {
                ShowExceptions = true
            });
            app.Verify(x => x.ApplicationServices, Times.Once);
        }

        [Fact]
        public void LaraCreateConnection()
        {
            using var app = new Application();
            app.CreateModeController(ApplicationMode.Default);
            var x = app.CreateConnection(IPAddress.Loopback);
            var ok = app.TryGetConnection(x.Id, out var y);
            Assert.True(ok);
            Assert.Same(x, y);
            app.ClearEmptyConnection(x);
            Assert.False(app.TryGetConnection(x.Id, out _));
        }

        [Fact]
        public void SetDefaultErrorPage()
        {
            var published = Context.Application.GetPublished();
            var x = new ErrorPages(published);
            var page = new MyPage();
            x.SetDefaultPage(HttpStatusCode.ExpectationFailed, () => page);
            var result = x.GetPage(HttpStatusCode.ExpectationFailed);
            var show = result.CreateInstance();
            Assert.Same(page, show);

            x.Remove(HttpStatusCode.ExpectationFailed);
            result = x.GetPage(HttpStatusCode.ExpectationFailed);
            Assert.Null(result);
        }

        [Fact]
        public void DefaultErrorPageReturned()
        {
            var published = Context.Application.GetPublished();
            var x = new ErrorPages(published);
            var page = x.DefaultServerError();
            Assert.True(page is DefaultErrorPage);
        }

        [Fact]
        public void DefaultNotFoundReturned()
        {
            var x = new ErrorPages(Context.Application.GetPublished());
            var page = x.DefaultNotFound();
            Assert.True(page is DefaultErrorPage);
        }

        [Fact]
        public void SequencerReorders()
        {
            var builder = new StringBuilder();
            var x = new Sequencer();
            Task.Run(async () =>
            {
                var ok = await x.WaitForTurn(2);
                Assert.True(ok);
                AddChars(builder, "a");
            });
            Task.Run(async () =>
            {
                var ok = await x.WaitForTurn(1);
                Assert.True(ok);
                AddChars(builder, "b");
            });
            Task.Run(async () =>
            {
                var ok = await x.WaitForTurn(3);
                Assert.True(ok);
                Assert.Equal("ba", builder.ToString());
            });
        }

        [Fact]
        public void SequencerAborts()
        {
            var x = new Sequencer();
            Task.Run(async () =>
            {
                var ok = await x.WaitForTurn(3);
                Assert.False(ok);
            });
            Task.Run(async () =>
            {
                var ok = await x.WaitForTurn(1);
                Assert.True(ok);
                x.AbortAll();
            });
        }

        private static void AddChars(StringBuilder builder, string text)
        {
            builder.Append(text);
        }

        [Fact]
        public void ApplicationModeControllerProperties()
        {
            using var app = new Application();
            app.CreateModeController(ApplicationMode.BrowserApp);
            Assert.Equal(BrowserAppController.BrowserAppKeepAliveInterval, app.KeepAliveInterval);
            Assert.True(app.AllowLocalhostOnly);
        }

        [Fact]
        public async void LocalhostFilterPass()
        {
            var http = new Mock<HttpContext>();
            var cnx = new Mock<ConnectionInfo>();
            http.Setup(x => x.Connection).Returns(cnx.Object);
            cnx.Setup(x => x.RemoteIpAddress).Returns(IPAddress.Loopback);
            var next = new Mock<RequestDelegate>();
            var logger = new Mock<ILogger<LocalhostFilter>>();
            var filter = new LocalhostFilter(next.Object, logger.Object);
            await filter.Invoke(http.Object);
            next.Verify(x => x.Invoke(http.Object), Times.Once);
        }

        [Fact]
        public void StartServerOptionsSettings()
        {
            var x = new StartServerOptions
            {
                Port = 12345,
                IPAddress = IPAddress.Loopback
            };
            Assert.Equal(12345, x.Port);
            Assert.Equal(IPAddress.Loopback, x.IPAddress);
        }

        private static readonly object _MyLock = new object();

        [Fact]
        public void LaraUiDefaultStatic()
        {
            lock (_MyLock)
            {
                LaraUI.Publish("/a", new StaticContent(Encoding.UTF8.GetBytes("hello"), "text"));
                Assert.True(LaraUI.DefaultApplication.TryGetNode("/a", out _));
            }
        }

        [Fact]
        public void LaraUiDefaultPage()
        {
            lock (_MyLock)
            {
                LaraUI.Publish("/b", () => new MyPage());
                Assert.True(LaraUI.DefaultApplication.TryGetNode("/b", out _));
                LaraUI.UnPublish("/b");
                Assert.False(LaraUI.DefaultApplication.TryGetNode("/b", out _));
            }
        }

        [Fact]
        public void LaraUiDefaultService()
        {
            lock (_MyLock)
            {
                LaraUI.Publish(new WebComponentOptions
                {
                    ComponentTagName = "lala-lala",
                    ComponentType = typeof(RemovableComponent)
                });
                Assert.True(LaraUI.DefaultApplication.TryGetComponent("lala-lala", out _));
                LaraUI.UnPublishWebComponent("lala-lala");
                Assert.False(LaraUI.DefaultApplication.TryGetComponent("lala-lala", out _));
            }
        }

        [Fact]
        public void NoCurrentSessionParameters()
        {
            var inner = new InvalidOperationException("x");
            var x = new NoCurrentSessionException("a", inner);
            Assert.Same(inner, x.InnerException);
            Assert.Equal("a", x.Message);
        }

        [Fact]
        public void NoCurrentSessionBase()
        {
            var x = new NoCurrentSessionException();
            Assert.Null(x.InnerException);
        }

        [Fact]
        public void LaraUiDocument()
        {
            var x = new Mock<IPageContext>();
            var doc = new Document(new MyPage(), 100);
            x.Setup(x1 => x1.Document).Returns(doc);
            LaraUI.InternalContext.Value = x.Object;
            Assert.Same(doc, LaraUI.Document);
            Assert.Null(LaraUI.GetContextDocument(null));
        }

        [Fact]
        public void SetExtraData()
        {
            var app = new Application();
            var http = new Mock<HttpContext>();
            using var connections = new Connections();
            var connection = connections.CreateConnection(IPAddress.Loopback);
            var x = new PageContext(app, http.Object, connection);
            x.SetExtraData("abc");
            Assert.Equal("abc", x.JSBridge.EventData);
            Assert.Same(connection.Session, x.Session);
        }

        [Fact]
        public async void StopStops()
        {
            var counter = 0;
            using var x = new Application();
            var host = new Mock<IWebHost>();
            x.SetHost(host.Object);
            var token = CancellationToken.None;
            host.Setup(x1 => x1.StopAsync(token)).Callback(() => counter++);

            await x.Stop(token);
            Assert.Equal(1, counter);
        }

        [Fact]
        public void ReuseConnection()
        {
            var app = Context.Application;
            var http = new Mock<HttpContext>();
            var request = new Mock<HttpRequest>();
            var cookies = new Mock<IRequestCookieCollection>();
            http.Setup(x => x.Request).Returns(request.Object);
            request.Setup(x => x.Cookies).Returns(cookies.Object);
            var info = new Mock<ConnectionInfo>();
            http.Setup(x => x.Connection).Returns(info.Object);
            info.Setup(x => x.RemoteIpAddress).Returns(IPAddress.Loopback);

            app.CreateModeController(ApplicationMode.Default);
            var current = app.CreateConnection(IPAddress.Loopback);
            var id = current.Id.ToString(GlobalConstants.GuidFormat, CultureInfo.InvariantCulture);
            cookies.Setup(x => x.TryGetValue(GlobalConstants.CookieSessionId, out id)).Returns(true);
            
            var connection = PagePublished.GetConnection(app, http.Object);
            Assert.Equal(current, connection);
        }

        [Fact]
        public async void PageStatusCodeReturned()
        {
            var page = new MyStatusPage();
            var http = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            var headers = new Mock<IHeaderDictionary>();
            var body = new Mock<Stream>();
            response.Setup(x => x.Headers).Returns(headers.Object);
            http.Setup(x => x.Response).Returns(response.Object);
            response.SetupProperty(x => x.StatusCode);
            response.Setup(x => x.Body).Returns(body.Object);
            var ok = await PagePublished.RunPage(Context.Application, http.Object, page, new LaraOptions());
            Assert.False(ok);
            Assert.Equal((int)HttpStatusCode.Unauthorized, http.Object.Response.StatusCode);
        }
        
        [Fact]
        public void ETagFormatCorrect()
        {
            const int hash = 12345;
            var expected = "\"" + hash.ToString(CultureInfo.InvariantCulture) + "\"";
            var etag = StaticContent.FormatETag(hash);
            Assert.Equal(expected, etag);
        }
    }
}
