/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;

namespace Integrative.Lara
{
    internal class AttributeObserver
    {
        private readonly HashSet<Action<string?>> _subscribers = new HashSet<Action<string?>>();

        public void Subscribe(Action<string?> handler) => _subscribers.Add(handler);

        public void Unsubscribe(Action<string?> handler) => _subscribers.Remove(handler);

        public void Dispatch(string? value)
        {
            foreach (var handler in _subscribers)
            {
                handler(value);
            }
        }
    }
}
