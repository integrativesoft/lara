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
    class MessageTypeRegistry
    {
        readonly HashSet<Func<Task>> _registry = new HashSet<Func<Task>>();

        public void Add(Func<Task> handler)
        {
            _registry.Add(handler);
        }

        public void Remove(Func<Task> handler)
        {
            _registry.Remove(handler);
        }

        public async Task RunAll()
        {
            var list = new List<Func<Task>>(_registry);
            foreach (var handler in list)
            {
                await handler();
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

        public void Add(string messageId, Func<Task> handler)
        {
            var registry = GetRegistry(messageId);
            registry.Add(handler);
        }

        private MessageTypeRegistry GetRegistry(string messageId)
        {
            if (!_map.TryGetValue(messageId, out var registry))
            {
                registry = new MessageTypeRegistry();
                _map.Add(messageId, registry);
                _parent.Head.On("_" + messageId, () =>
                {
                    return RunAll(messageId);
                });
            }

            return registry;
        }

        public void Remove(string messageId, Func<Task> handler)
        {
            if (_map.TryGetValue(messageId, out var registry))
            {
                registry.Remove(handler);
            }
        }

        public async Task RunAll(string messageId)
        {
            if (_map.TryGetValue(messageId, out var registry))
            {
                await registry.RunAll();
            }
        }
    }
}
