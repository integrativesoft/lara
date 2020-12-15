/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;

namespace Integrative.Lara
{
    /// <summary>
    /// JSON operations
    /// </summary>
    public sealed class LaraJson
    {
        /// <summary>
        /// Parses a JSON string into a class. The class needs to be decorated with the DataContract attribute.
        /// </summary>
        /// <typeparam name="T">Class type</typeparam>
        /// <param name="json">Source JSON string</param>
        /// <param name="result">Class instance created</param>
        /// <returns>true when successful, false otherwise</returns>
        // ReSharper disable once MemberCanBeMadeStatic.Global
        public bool TryParse<T>(string json, [NotNullWhen(true)] out T? result) where T : class
        {
            try
            {
                result = LaraTools.Deserialize<T>(json);
                return (result != null);
            }
            catch (SerializationException)
            {
                result = default;
                return false;
            }
        }

        /// <summary>
        /// Parses a JSON string. If parsing fails, throws a StatusCodeException that returns a Bad Request (400).
        /// The class needs to be decorated with the DataContract attribute.
        /// </summary>
        /// <typeparam name="T">Class type</typeparam>
        /// <param name="json">JSON source text</param>
        /// <returns>Instance of deserialized class</returns>
        public T Parse<T>(string json) where T : class
        {
            T? result;
            try
            {
                result = LaraTools.Deserialize<T>(json);
            }
            catch (Exception e)
            {
                var outer = new StatusCodeException(Resources.BadRequest, e)
                {
                    StatusCode = HttpStatusCode.BadRequest
                };
                throw outer;
            }
            if (result == null)
            {
                throw new StatusCodeException(HttpStatusCode.BadRequest, Resources.BadRequest);
            }
            return result;
        }

        /// <summary>
        /// Serializes a class decorated with DataContract to a JSON string
        /// </summary>
        /// <typeparam name="T">Type of class</typeparam>
        /// <param name="instance">Instance to serialize</param>
        /// <returns>JSON string</returns>
        public string Stringify<T>(T instance) => LaraTools.Serialize(instance);

        /// <summary>
        /// Serializes a class decorated with DataContract to a JSON string
        /// </summary>
        /// <param name="instance">Instance to serialize</param>
        /// <param name="type">Type of class</param>
        /// <returns>JSON string</returns>
        public string Stringify(object instance, Type type) => LaraTools.Serialize(instance, type);

        internal LaraJson()
        {
        }
    }
}
