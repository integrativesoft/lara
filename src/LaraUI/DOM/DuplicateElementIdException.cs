/*
Copyright (c) 2019-2021 Integrative Software LLC
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
    public class DuplicateElementIdException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateElementIdException"/> class.
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public DuplicateElementIdException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateElementIdException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        // ReSharper disable once MemberCanBePrivate.Global
        public DuplicateElementIdException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateElementIdException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public DuplicateElementIdException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Creates a DuplicateElementId exception
        /// </summary>
        /// <param name="id">The identifier that is duplicate.</param>
        /// <returns>Exception created</returns>
        public static DuplicateElementIdException Create(string id)
        {
            var message = $"Duplicate element Id: {id}";
            return new DuplicateElementIdException(message);
        }
    }
}
