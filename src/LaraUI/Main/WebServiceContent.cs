/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Web server content definitions
    /// </summary>
    public sealed class WebServiceContent
    {
        /// <summary>
        /// The URL of the web service (e.g. '/MyService')
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// The HTTP method (e.g. 'POST')
        /// </summary>
        public string Method { get; set; } = "POST";

        /// <summary>
        /// The web service's reponse content-type (e.g. 'application/json')
        /// </summary>
        public string ContentType { get; set; } = "application/json";

        /// <summary>
        /// The method for creating instances of the web service class
        /// </summary>
        public Func<IWebService>? Factory { get; set; }

        internal Func<IWebService> GetFactory()
        {
            return Factory ?? throw new MissingMemberException(nameof(WebServiceContent), nameof(Factory));
        }
    }
}
