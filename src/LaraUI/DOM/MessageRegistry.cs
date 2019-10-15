/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Parameters for custom JavaScript events
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Text body of the custom JavaScript event
        /// </summary>
        public string Body { get; }

        internal MessageEventArgs(string body)
        {
            Body = body;
        }
    }

    class MessageTypeRegistry
    {
        readonly HashSet<Func<MessageEventArgs, Task>> _registry = new HashSet<Func<MessageEventArgs, Task>>();

        public void Add(Func<MessageEventArgs, Task> handler)
        {
            _registry.Add(handler);
        }

        public void Remove(Func<MessageEventArgs, Task> handler)
        {
            _registry.Remove(handler);
        }

        public async Task RunAll(string body)
        {
            var args = new MessageEventArgs(body);
            foreach (var handler in _registry)
            {
                await handler(args);
            }
        }
    }

    class MessageRegistry
    {
        readonly Document _parent;
        readonly Dictionary<string, MessageTypeRegistry> _map;

        public MessageRegistry(Document parent)
        {
            _parent = parent;
            _map = new Dictionary<string, MessageTypeRegistry>();
        }

        public void Add(string messageId, Func<MessageEventArgs, Task> handler)
        {
            if (!_map.TryGetValue(messageId, out var registry))
            {
                registry = new MessageTypeRegistry();
                _map.Add(messageId, registry);
                _parent.Head.On("_" + messageId, () =>
                {
                    var body = LaraUI.Page.JSBridge.EventData;
                    return RunAll(messageId, body);
                });
            }
            registry.Add(handler);
        }

        public void Remove(string messageId, Func<MessageEventArgs, Task> handler)
        {
            if (_map.TryGetValue(messageId, out var registry))
            {
                registry.Remove(handler);
            }
        }

        public async Task RunAll(string messageId, string body)
        {
            if (_map.TryGetValue(messageId, out var registry))
            {
                await registry.RunAll(body);
            }
        }
    }
}
