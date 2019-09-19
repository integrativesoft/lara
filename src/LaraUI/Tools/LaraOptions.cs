/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Diagnostics;

namespace Integrative.Lara
{
    /// <summary>
    /// Lara Web Engine options
    /// </summary>
    public class LaraOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to allow localhost requests only.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [allow localhost only]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowLocalhostOnly { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether Lara will show its default 'not found' page.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show not found page]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowNotFoundPage { get; set; } = true;

        /// <summary>
        /// Gets or sets the port to use when Lara opens a new server. If zero, a dynamic port wil be assigned.
        /// </summary>
        /// <value>
        /// The port number.
        /// </value>
        public int Port { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether Lara will include ASP.NET Core WebSocket middleware. Always set to include unless you are manually including the middleware yourself.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [add web sockets middleware]; otherwise, <c>false</c>.
        /// </value>
        public bool AddWebSocketsMiddleware { get; set; } = true;

        /// <summary>
        /// Default address for JQuery.js
        /// </summary>
        public const string DefaultJQuery = "https://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.4.1.min.js";

        private Uri _jquery;

        internal string AddressJQuery { get; private set; } = DefaultJQuery;

        /// <summary>
        /// Specifies a custom URL for the JQuery.js library. If left blank, the default URL is used.
        /// </summary>
        public Uri CustomUrlJQuery
        {
            get => _jquery;
            set
            {
                _jquery = value;
                if (value == null)
                {
                    AddressJQuery = DefaultJQuery;
                }
                else
                {
                    AddressJQuery = value.ToString();
                }
            }
        }
    }

    /// <summary>
    /// Lara options for starting a web server
    /// </summary>
    /// <seealso cref="LaraOptions" />
    public class StartServerOptions : LaraOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show execution exceptions.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show exceptions]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowExceptions { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartServerOptions"/> class.
        /// </summary>
        public StartServerOptions() : base()
        {
            ShowExceptions = Debugger.IsAttached;
        }
    }
}
