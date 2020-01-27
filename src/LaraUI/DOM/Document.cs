/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Status options for server-side events
    /// </summary>
    public enum ServerEventsStatus
    {
        /// <summary>
        /// Server-side events have not been enabled
        /// </summary>
        Disabled,

        /// <summary>
        /// The server is waiting for the client to listen to server-side events
        /// </summary>
        Connecting,

        /// <summary>
        /// Server-side events are enabled
        /// </summary>
        Enabled
    }

    /// <summary>
    /// An HTML5 document.
    /// </summary>
    public class Document
    {
        internal IPage Page { get; }

        /// <summary>
        /// Global unique identifier for the document
        /// </summary>
        public Guid VirtualId { get; }

        private readonly DocumentIdMap _map;
        private readonly Queue<BaseDelta> _queue;
        internal SemaphoreSlim Semaphore { get; }

        private readonly ServerEventsController _serverEvents;
        private readonly MessageRegistry _messageRegistry;
        private readonly Sequencer _sequencer = new Sequencer();

        internal Dictionary<string, EventSettings> Events { get; } = new Dictionary<string, EventSettings>();

        private int _serializer;

        /// <summary>
        /// Occurs when the document is unloaded
        /// </summary>
        public event EventHandler? OnUnload;

        /// <summary>
        /// Asynchronous unload event
        /// </summary>
        public AsyncEvent OnUnloadAsync { get; } = new AsyncEvent();

        internal event EventHandler? AfterUnload;

        /// <summary>
        /// Gets or sets the language. See 'lang' property for HTML5 documents.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string? Lang { get; set; }

        /// <summary>
        /// The document's Head element.
        /// </summary>
        /// <value>
        /// The head.
        /// </value>
        public HeadElement Head { get; }

        /// <summary>
        /// The document's Body element.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public BodyElement Body { get; }

        internal DateTime LastUtc { get; private set; }

        internal Queue<BaseDelta> GetQueue() => _queue;

        internal Document(IPage page, double keepAliveInterval)
            : this(page, Connections.CreateCryptographicallySecureGuid(), keepAliveInterval)
        {
        }

        internal Document(IPage page, Guid virtualId, double keepAliveInterval)
        {
            VirtualId = virtualId;
            Page = page;
            _map = new DocumentIdMap();
            _queue = new Queue<BaseDelta>();
            Semaphore = new SemaphoreSlim(1);
            Head = new HeadElement
            {
                Document = this,
                IsSlotted = true
            };
            Body = new BodyElement
            {
                Document = this,
                IsSlotted = true
            };
            UpdateTimestamp();
            TemplateBuilder.Build(this, keepAliveInterval);
            _serverEvents = new ServerEventsController(this);
            _messageRegistry = new MessageRegistry(this);
        }

        internal string VirtualIdString =>
            VirtualId.ToString(GlobalConstants.GuidFormat, CultureInfo.InvariantCulture);

        /// <summary>
        /// Creates an HTML element.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns>Element created</returns>
        public static Element CreateElement(string tagName) => Element.Create(tagName);

        /// <summary>
        /// Creates a text node.
        /// </summary>
        /// <param name="data">The node's data.</param>
        /// <returns>Text node created</returns>
        public static TextNode CreateTextNode(string data) => new TextNode(data);

        internal void UpdateTimestamp()
        {
            LastUtc = DateTime.UtcNow;
        }

        internal void ModifyLastUtcForTesting(DateTime value)
        {
            LastUtc = value;
        }

        internal string GenerateElementId()
        {
            _serializer++;
            return "_e" + _serializer.ToString(CultureInfo.InvariantCulture);
        }

        internal void OnElementAdded(Element element)
            => _map.NotifyAdded(element);

        internal void OnElementRemoved(Element element)
            => _map.NotifyRemoved(element);

        internal void NotifyChangeId(Element element, string? before, string? after)
            => _map.NotifyChangeId(element, before, after);

        internal bool QueueingEvents { get; private set; }

        internal void OpenEventQueue()
        {
            if (_queue.Count > 0)
            {
                var json = FlushQueue();
                Head.SetAttribute("data-lara-initialdelta", json);
            }
            QueueingEvents = true;
        }

        internal string FlushQueue()
        {
            var list = new List<BaseDelta>();
            while (_queue.Count > 0)
            {
                var step = _queue.Dequeue();
                list.Add(step);
            }
            var result = new EventResult(list);
            return result.ToJSON();
        }

        internal void Enqueue(BaseDelta delta)
        {
            _queue.Enqueue(delta);
        }

        /// <summary>
        /// Retrieves the element with the given ID.
        /// </summary>
        /// <param name="id">Element ID</param>
        /// <param name="element">The element.</param>
        /// <returns>True when the element was found.</returns>
        public bool TryGetElementById(string id, out Element element)
            => _map.TryGetElementById(id, out element);

        /// <summary>
        /// Retrieves the element with the given ID.
        /// </summary>
        /// <param name="id">Element ID</param>
        /// <returns>The element</returns>
        public Element GetElementById(string id)
        {
            _map.TryGetElementById(id, out var element);
            return element;
        }

        /// <summary>
        /// Returns true when there are UI changes pending to be flushed to the client
        /// </summary>
        public bool HasPendingChanges => _queue.Count > 0;

        internal void OnMessage(string key, Func<Task> handler)
        {
            Head.On("_" + key, handler);
        }

        internal void AddMessageListener(string messageId, Func<MessageEventArgs, Task> handler)
        {
            _messageRegistry.Add(messageId, handler);
        }

        internal void RemoveMessageListener(string messageId, Func<MessageEventArgs, Task> handler)
        {
            _messageRegistry.Remove(messageId, handler);
        }

        internal async Task NotifyUnload()
        {
            await _serverEvents.NotifyUnload();
            var args = new EventArgs();
            OnUnload?.Invoke(this, args);
            await OnUnloadAsync.InvokeAsync(this, args);
            AfterUnload?.Invoke(this, new EventArgs());
            _sequencer.AbortAll();
        }

        /// <summary>
        /// Returns the current status of server-side events
        /// </summary>
        public ServerEventsStatus ServerEventsStatus
            => _serverEvents.ServerEventsStatus;

        /// <summary>
        /// Starts a server event. Call with 'using' and dispose the class returned.
        /// </summary>
        /// <returns>Disposable token</returns>
        public ServerEvent StartServerEvent()
            => _serverEvents.StartServerEvent();

        internal void ServerEventsOn()
        {
            NotifyHasEvent();
            _serverEvents.ServerEventsOn();
        }

        internal Task ServerEventsOff()
            => _serverEvents.ServerEventsOff();

        internal bool SocketRemainsOpen(string eventName)
            => _serverEvents.SocketRemainsOpen(eventName);

        internal virtual Task<TaskCompletionSource<bool>> GetSocketCompletion(WebSocket socket)
            => _serverEvents.GetSocketCompletion(socket);

        internal Task ServerEventFlush()
            => _serverEvents.ServerEventFlush();

        internal ServerEventsController GetServerEventsController()
            => _serverEvents;

        internal Task<bool> WaitForTurn(long turn) => _sequencer.WaitForTurn(turn);

        internal bool CanDiscard { get; private set; } = true;

        internal void NotifyHasEvent()
        {
            CanDiscard = false;
        }

        internal Task NotifyEvent(string eventName)
        {
            if (Events.TryGetValue(eventName, out var settings)
                && settings.Handler != null)
            {
                return settings.Handler();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Registers an event and associates code to execute.
        /// </summary>
        /// <param name="settings">The event's settings.</param>
        public void On(EventSettings settings)
        {
            settings = settings ?? throw new ArgumentNullException(nameof(settings));
            settings.Verify();
            RemoveEvent(settings.EventName);
            Events.Add(settings.EventName, settings);
            SubscribeDelta.Enqueue(this, settings);
        }

        /// <summary>
        /// Registers an event and associates code to execute.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="handler">The handler to execute.</param>
        public void On(string eventName, Func<Task>? handler)
        {
            eventName = eventName ?? throw new ArgumentNullException(nameof(eventName));
            if (handler == null)
            {
                RemoveEvent(eventName);
            }
            else
            {
                On(new EventSettings
                {
                    EventName = eventName,
                    Handler = handler
                });
            }
        }

        private void RemoveEvent(string eventName)
        {
            if (Events.ContainsKey(eventName))
            {
                Events.Remove(eventName);
                Enqueue(new UnsubscribeDelta
                {
                    ElementId = string.Empty,
                    EventName = eventName
                });
            }
        }

    }
}
