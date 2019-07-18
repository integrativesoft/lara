/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Exception thrown when adding a duplicate element ID into a document
    /// </summary>
    /// <seealso cref="System.InvalidOperationException" />
    public class DuplicateElementId : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateElementId"/> class.
        /// </summary>
        public DuplicateElementId()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateElementId"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DuplicateElementId(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateElementId"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DuplicateElementId(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Creates a DuplicateElementId exception
        /// </summary>
        /// <param name="id">The identifier that is duplicate.</param>
        /// <returns>Exception created</returns>
        public static DuplicateElementId Create(string id)
        {
            string message = $"Duplicate element Id: {id}";
            return new DuplicateElementId(message);
        }
    }
}
