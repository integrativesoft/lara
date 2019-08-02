/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// The execution context for events.
    /// </summary>
    public interface IPageContext
    {
        /// <summary>
        /// Gets the .NET Core HttpContext instance
        /// </summary>
        HttpContext Http { get; }

        /// <summary>
        /// Gets the current document.
        /// </summary>
        Document Document { get; }

        /// <summary>
        /// Bridge to execute JavaScript on the client
        /// </summary>
        IJSBridge JSBridge { get; }

        /// <summary>
        /// Methods related to navigation
        /// </summary>
        INavigation Navigation { get; }

        /// <summary>
        /// Session tools
        /// </summary>
        Session Session { get; }
    }

    /// <summary>
    /// Bridge to execute JavaScript on the client
    /// </summary>
    public interface IJSBridge
    {
        /// <summary>
        /// Submits the specified java script code to execute on the client. The code is executed after the current event finishes on the server and control returns to the client.
        /// </summary>
        /// <param name="javaScriptCode">The JavaScript code to execute.</param>
        void Submit(string javaScriptCode);

        /// <summary>
        /// Register a custom event that can be called from JavaScript code
        /// </summary>
        /// <param name="key">The key for the event.</param>
        /// <param name="handler">The handler for the event.</param>
        void OnMessage(string key, Func<IPageContext, Task> handler);

        /// <summary>
        /// Gets extra event data that can be passed by the client on custom events.
        /// </summary>
        /// <value>
        /// The event data.
        /// </value>
        string EventData { get; }

        /// <summary>
        /// Makes the client start listening for ServerEventFlush() notifications.
        /// </summary>
        void ServerEventsOn();

        /// <summary>
        /// Makes the client stop listening for ServerEventFlush() notifications.
        /// </summary>
        Task ServerEventsOff();
    }
}
