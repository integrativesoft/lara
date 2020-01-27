/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Marks a class as a web service that replies an array of bytes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class LaraBinaryServiceAttribute : Attribute
    {
        /// <summary>
        /// Web Service's address (e.g. '/myWS')
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Web Service's method (e.g. 'POST')
        /// </summary>
        public string Method { get; set; } = "POST";

        /// <summary>
        /// Web Service's response content-type HTTP header (e.g. 'image/gif')
        /// </summary>
        public string ContentType { get; set; } = string.Empty;
    }
}
