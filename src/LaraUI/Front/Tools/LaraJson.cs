﻿/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tools;
using System;
using System.Runtime.Serialization;

namespace Integrative.Lara
{
    /// <summary>
    /// JSON operations
    /// </summary>
    public sealed class LaraJson
    {
        /// <summary>
        /// Parses a JSON string into a class
        /// </summary>
        /// <typeparam name="T">Class type</typeparam>
        /// <param name="json">Source JSON string</param>
        /// <param name="result">Class instance created</param>
        /// <returns>true when successful, false otherwise</returns>
        public bool TryParse<T>(string json, out T result) where T : class
        {
            try
            {
                result = LaraTools.Deserialize<T>(json);
                return true;
            }
            catch (SerializationException)
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Parses a JSON string. If parsing fails, throws a StatusCodeException that returns a Bad Request (400).
        /// </summary>
        /// <typeparam name="T">Class type</typeparam>
        /// <param name="json">JSON source text</param>
        /// <returns>Instance of deserialized class</returns>
        public T Parse<T>(string json) where T : class
        {
            try
            {
                return LaraTools.Deserialize<T>(json);
            }
            catch (Exception e)
            {
                var outer = new StatusCodeException("Bad request", e)
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                };
                throw outer;
            }
        }

        /// <summary>
        /// Serializes a class decorated with DataContract to a JSON string
        /// </summary>
        /// <typeparam name="T">Type of class</typeparam>
        /// <param name="instance">Instance to serialize</param>
        /// <returns>JSON string with serialized class</returns>
        public string Stringify<T>(T instance) => LaraTools.Serialize<T>(instance);

        internal LaraJson()
        {
        }
    }
}