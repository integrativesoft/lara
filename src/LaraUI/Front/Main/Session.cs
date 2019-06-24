/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Session information
    /// </summary>
    public sealed class Session
    {
        readonly Connection _connection;

        internal Session(Connection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Returns an ID that uniquely identifies the UI session
        /// </summary>
        public Guid SessionId => _connection.Id;

        /// <summary>
        /// Stores a key value pair
        /// </summary>
        /// <param name="key">Identifier of the value to store</param>
        /// <param name="value">Value to store</param>
        public void SaveValue(string key, string value)
            => _connection.Storage.Save(key, value);

        /// <summary>
        /// Removes a stored value
        /// </summary>
        /// <param name="key">Identifier of the value stored</param>
        public void RemoveValue(string key)
            => _connection.Storage.Remove(key);

        /// <summary>
        /// Retrieves a value stored
        /// </summary>
        /// <param name="key">Identifier of the value stored</param>
        /// <param name="value">Value stored</param>
        /// <returns>true if found, false otherwise</returns>
        public bool TryGetValue(string key, out string value)
            => _connection.Storage.TryGetValue(key, out value);
    }
}
