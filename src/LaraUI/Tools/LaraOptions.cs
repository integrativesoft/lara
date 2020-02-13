/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Builder;

namespace Integrative.Lara
{
    /// <summary>
    /// Defines preset modes for applications
    /// </summary>
    public enum ApplicationMode
    {
        /// <summary>
        /// Default mode as a regular web site
        /// </summary>
        Default,

        /// <summary>
        /// Launches a browser tab and terminates the host when the user closes away the tab and its child tabs.
        /// Also, will prevent any further connections besides the one created upon launching the browser.
        /// </summary>
        BrowserApp
    }

    /// <summary>
    /// Lara Web Engine options
    /// </summary>
    public class LaraOptions
    {
        /// <summary>
        /// When set to true, upon starting the application the assemblies will be scanned for classes decorated with 'Lara' attributes and publish them.
        /// </summary>
        public bool PublishAssembliesOnStart { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to allow localhost requests only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow localhost only]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowLocalhostOnly { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Lara will show its default 'not found' page.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show not found page]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowNotFoundPage { get; set; } = true;

        /// <summary>
        /// Defines an application mode
        /// </summary>
        public ApplicationMode Mode { get; set; } = ApplicationMode.Default;

        /// <summary>
        /// Gets or sets a value indicating whether Lara will include ASP.NET Core WebSocket middleware. Always set to include unless you are manually including the middleware yourself.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [add web sockets middleware]; otherwise, <c>false</c>.
        /// </value>
        public bool AddWebSocketsMiddleware { get; set; } = true;
    }

    /// <summary>
    /// Lara options for starting a web server
    /// </summary>
    /// <seealso cref="LaraOptions" />
    public class StartServerOptions : LaraOptions
    {
        /// <summary>
        /// Gets or sets the port to use when Lara opens a new server. If zero, a dynamic port wil be assigned.
        /// </summary>
        /// <value>
        /// The port number.
        /// </value>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the IP address where the host is listening. By default, this is the loopback address.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public IPAddress IPAddress { get; set; } = IPAddress.Loopback;

        /// <summary>
        /// Gets or sets a value indicating whether to show execution exceptions.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show exceptions]; otherwise, <c>false</c>.
        /// </value>
        // ReSharper disable once MemberCanBePrivate.Global
        public bool ShowExceptions { get; set; }

        /// <summary>
        /// Defines an optional method to set additional configuration for the asp.net core instance
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Action<IApplicationBuilder>? AdditionalConfiguration { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StartServerOptions"/> class.
        /// </summary>
        public StartServerOptions()
        {
            ShowExceptions = Debugger.IsAttached;
        }
    }
}
