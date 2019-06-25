/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System;
using System.Net;

namespace Integrative.Lara.Front.Middleware
{
    /// <summary>
    /// Exception that returns a status code of Forbidden in web services
    /// </summary>
    public class StatusForbiddenException : StatusCodeException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public StatusForbiddenException()
            : base(HttpStatusCode.Forbidden)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        public StatusForbiddenException(string message)
            : base(HttpStatusCode.Forbidden, message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="inner">Inner exception</param>
        public StatusForbiddenException(string message, Exception inner)
            : base(message, inner)
        {
            StatusCode = HttpStatusCode.Forbidden;
        }
    }
}
