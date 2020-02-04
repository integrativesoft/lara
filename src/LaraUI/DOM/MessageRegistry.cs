/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Parameters for client messages
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Body of message sent from JavaScript
        /// </summary>
        public string? Body { get; }

        internal MessageEventArgs(string? body)
        {
            Body = body;
        }
    }

    internal class MessageTypeRegistry
    {
        private readonly HashSet<Func<MessageEventArgs, Task>> _registry = new HashSet<Func<MessageEventArgs, Task>>();

        public void Add(Func<MessageEventArgs, Task> handler)
        {
            _registry.Add(handler);
        }

        public void Remove(Func<MessageEventArgs, Task> handler)
        {
            _registry.Remove(handler);
        }

        public async Task RunAll(MessageEventArgs args)
        {
            var list = new List<Func<MessageEventArgs, Task>>(_registry);
            foreach (var handler in list)
            {
                await handler(args);
            }
        }
    }

    internal class MessageRegistry
    {
        private readonly Document _parent;
        private readonly Dictionary<string, MessageTypeRegistry> _map;

        public MessageRegistry(Document parent)
        {
            _parent = parent;
            _map = new Dictionary<string, MessageTypeRegistry>();
        }

        public void Add(string messageId, Func<MessageEventArgs, Task> handler)
        {
            var registry = GetRegistry(messageId);
            registry.Add(handler);
        }

        private MessageTypeRegistry GetRegistry(string messageId)
        {
            if (_map.TryGetValue(messageId, out var registry)) return registry;
            registry = new MessageTypeRegistry();
            _map.Add(messageId, registry);
            _parent.Head.On("_" + messageId, () =>
            {
                var body = LaraUI.Page.JSBridge.EventData;
                var args = new MessageEventArgs(body);
                return RunAll(messageId, args);
            });

            return registry;
        }

        public void Remove(string messageId, Func<MessageEventArgs, Task> handler)
        {
            if (_map.TryGetValue(messageId, out var registry))
            {
                registry.Remove(handler);
            }
        }

        public async Task RunAll(string messageId, MessageEventArgs args)
        {
            if (_map.TryGetValue(messageId, out var registry))
            {
                await registry.RunAll(args);
            }
        }
    }
}
