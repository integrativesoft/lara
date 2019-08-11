﻿/*
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
    public sealed class LaraWebComponent : Attribute
    {
        /// <summary>
        /// Custom tag name for the component. Must contain the '-' character.
        /// </summary>
        public string ComponentTagName { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LaraWebComponent()
        {
        }

        /// <summary>
        /// Constructor with custom tag name
        /// </summary>
        /// <param name="customTagName"></param>
        public LaraWebComponent(string customTagName)
        {
            ComponentTagName = customTagName;
        }
    }
}
