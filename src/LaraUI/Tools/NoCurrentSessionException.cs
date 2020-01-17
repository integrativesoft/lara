/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// The operation requested requires a current session and there isn't one
    /// </summary>
    public class NoCurrentSessionException : InvalidOperationException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        public NoCurrentSessionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Inner exception</param>
        public NoCurrentSessionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public NoCurrentSessionException()
        {
        }
    }
}
