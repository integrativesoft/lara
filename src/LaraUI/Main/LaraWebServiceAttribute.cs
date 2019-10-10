/*
Copyright (c) 2019 Integrative Software LLC
Created: 7/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Declares a class as a web service that gets published with LaraUI.PublishAssemblies()
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class LaraWebServiceAttribute : Attribute
    {
        /// <summary>
        /// Web Service's address (e.g. '/myWS')
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Web Service's method (e.g. 'POST')
        /// </summary>
        public string Method { get; set; } = "POST";

        /// <summary>
        /// Web Service's response content-type HTTP header (e.g. 'application/json')
        /// </summary>
        public string ContentType { get; set; } = "application/json";
    }
}
