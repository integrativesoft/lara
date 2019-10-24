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
    /// Shared interface for both IPageContext and IWebServiceContext
    /// </summary>
    public interface ILaraContext
    {
        /// <summary>
        /// Gets the .NET Core HttpContext instance
        /// </summary>
        HttpContext Http { get; }
    }

    /// <summary>
    /// The execution context for events.
    /// </summary>
    public interface IPageContext : ILaraContext
    {
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
        /// <param name="payload">Optional payload to send to the client</param>
        void Submit(string javaScriptCode, string payload = null);

        /// <summary>
        /// Register a custom event that can be called from JavaScript code
        /// </summary>
        /// <param name="messageId">Message type identifier</param>
        /// <param name="handler">The handler for the event.</param>
        [Obsolete("Use instead AddMessageListener() and RemoveMessageListener().")]
        void OnMessage(string messageId, Func<Task> handler);

        /// <summary>
        /// Register a listener to a custom event that can be called from JavaScript code
        /// </summary>
        /// <param name="messageId">Message type identifier</param>
        /// <param name="handler">Handler to execute</param>
        void AddMessageListener(string messageId, Func<MessageEventArgs, Task> handler);

        /// <summary>
        /// Unregister a listener to a custom event called from JavaScript code
        /// </summary>
        /// <param name="messageId">Message type identifier</param>
        /// <param name="handler">Handler to execute</param>
        void RemoveMessageListener(string messageId, Func<MessageEventArgs, Task> handler);

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
