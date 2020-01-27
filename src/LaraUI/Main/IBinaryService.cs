/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Binary Service handler class
    /// </summary>
    public interface IBinaryService
    {
        /// <summary>
        /// Executes the web service
        /// </summary>
        /// <returns>Response's body</returns>
        Task<byte[]> Execute();
    }
}
