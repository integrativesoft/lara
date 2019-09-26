/*
Copyright (c) 2019 Integrative Software LLC
Created: 9/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Delegate for asynchronous events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate Task AsyncEventHandler<T>(object sender, T args) where T : EventArgs;

    /// <summary>
    /// Asynchronous event source
    /// </summary>
    public class AsyncEvent : AsyncEvent<EventArgs>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AsyncEvent() : base()
        {
        }
    }

    /// <summary>
    /// Generic asynchronous event source
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AsyncEvent<T> where T : EventArgs
    {
        private readonly HashSet<AsyncEventHandler<T>> _handlers;
        private readonly HashSet<AsyncEvent<T>> _batons;
        private readonly HashSet<Func<Task>> _actions;
        private readonly HashSet<Action> _voids;

        /// <summary>
        /// Constructor
        /// </summary>
        public AsyncEvent()
        {
            _handlers = new HashSet<AsyncEventHandler<T>>();
            _batons = new HashSet<AsyncEvent<T>>();
            _actions = new HashSet<Func<Task>>();
            _voids = new HashSet<Action>();
        }

        /// <summary>
        /// Adds an action to execute when this event is fired
        /// </summary>
        /// <param name="action">action to execute</param>
        public void Subscribe(Func<Task> action)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));
            _actions.Add(action);
        }

        /// <summary>
        /// Adds a method to execute when the event is fired
        /// </summary>
        /// <param name="handler">delegate</param>
        public void Subscribe(AsyncEventHandler<T> handler)
        {
            handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _handlers.Add(handler);
        }

        /// <summary>
        /// Adds an event to trigger when this event is fired
        /// </summary>
        /// <param name="other">event to trigger</param>
        public void Subscribe(AsyncEvent<T> other)
        {
            other = other ?? throw new ArgumentNullException(nameof(other));
            _batons.Add(other);
        }

        /// <summary>
        /// Unsubscribes a previously subscribed handler
        /// </summary>
        /// <param name="handler">handler</param>
        public void Unsubscribe(AsyncEventHandler<T> handler)
        {
            handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _handlers.Remove(handler);
        }

        /// <summary>
        /// Unsubscribes a previously subscribed handler
        /// </summary>
        /// <param name="handler">handler</param>
        public void Unsubscribe(AsyncEvent<T> handler)
        {
            handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _batons.Remove(handler);
        }

        /// <summary>
        /// Unsubscribes a previously subscribed action
        /// </summary>
        /// <param name="action"></param>
        public void Unsubscribe(Func<Task> action)
        {
            action = action ?? throw new ArgumentNullException(nameof(action));
            _actions.Remove(action);
        }

        /// <summary>
        /// Invokes an event
        /// </summary>
        /// <param name="sender">Event's source</param>
        /// <param name="args">Event's arguments</param>
        /// <returns>Task</returns>
        public async Task InvokeAsync(object sender, T args)
        {
            foreach (var handler in _handlers)
            {
                await handler(sender, args);
            }
            foreach (var baton in _batons)
            {
                await baton.InvokeAsync(sender, args);
            }
            foreach (var action in _actions)
            {
                await action();
            }
            foreach (var method in _voids)
            {
                method();
            }
        }
    }
}
