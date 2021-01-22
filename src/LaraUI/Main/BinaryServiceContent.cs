/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Binary web service content definitions
    /// </summary>
    public sealed class BinaryServiceContent
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
        /// The web service's reponse content-type (e.g. 'image/gif')
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// The method for creating instances of the web service class
        /// </summary>
        public Func<IBinaryService>? Factory { get; set; }

        internal Func<IBinaryService> GetFactory()
        {
            return Factory ?? throw new MissingMemberException(nameof(BinaryServiceContent), nameof(Factory));
        }
    }
}
