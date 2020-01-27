/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Methods related to document navigation
    /// </summary>
    public interface INavigation
    {
        /// <summary>
        /// Replaces the specified location.
        /// </summary>
        /// <param name="location">The new URL location.</param>
        void Replace(string location);

        /// <summary>
        /// Flushes the partial changes on the document to the client. Useful to report progress. Use with 'await'.
        /// </summary>
        /// <returns>Task</returns>
        Task FlushPartialChanges();
    }
}
