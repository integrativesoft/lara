/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System;
using System.Net;

namespace Integrative.Lara
{
    /// <summary>
    /// Exception that returns a specific error code on web services
    /// </summary>
    public class StatusCodeException : Exception
    {
        /// <summary>
        /// Status code to respond to the client
        /// </summary>
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;

        /// <summary>
        /// Constructor
        /// </summary>
        public StatusCodeException() : base()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public StatusCodeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception</param>
        public StatusCodeException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status">Status code</param>
        public StatusCodeException(HttpStatusCode status)
        {
            StatusCode = status;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status">Status code </param>
        /// <param name="message"></param>
        public StatusCodeException(HttpStatusCode status, string message)
            : base(message)
        {
            StatusCode = status;
        }
    }
}
