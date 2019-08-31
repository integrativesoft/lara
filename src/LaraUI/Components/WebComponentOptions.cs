/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Options for publishing web components
    /// </summary>
    public sealed class WebComponentOptions
    {
        /// <summary>
        /// Custom tag name for the component. Needs to include the '-' character.
        /// </summary>
        public string ComponentTagName { get; set; }

        /// <summary>
        /// Type of the component. Needs to inherit from WebComponent. Example: 'typeof(MyComponent)'
        /// </summary>
        public Type ComponentType { get; set; }
    }
}
