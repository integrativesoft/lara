/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Classes marked as LaraWebComponent will registered when executing LaraUI.PublishAssemblies()
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class LaraWebComponentAttribute : Attribute
    {
        /// <summary>
        /// Custom tag name for the component. Must contain the '-' character.
        /// </summary>
        public string ComponentTagName { get; }

        /// <summary>
        /// Constructor with custom tag name
        /// </summary>
        /// <param name="customTagName"></param>
        public LaraWebComponentAttribute(string customTagName)
        {
            ComponentTagName = customTagName;
        }
    }
}
