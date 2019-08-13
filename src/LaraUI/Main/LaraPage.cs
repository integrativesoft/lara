/*
Copyright (c) 2019 Integrative Software LLC
Created: 7/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Declares a class as a web page that gets published with LaraUI.PublishAssemblies()
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class LaraPage : Attribute
    {
        /// <summary>
        /// Page's address (e.g. '/myPage')
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LaraPage()
        {
        }

        /// <summary>
        /// Constuctor with address
        /// </summary>
        /// <param name="address">Page's address</param>
        public LaraPage(string address)
            : this()
        {
            Address = address;
        }
    }
}
