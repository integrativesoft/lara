/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Web Service handler class
    /// </summary>
    public interface IWebService
    {
        /// <summary>
        /// Executes the web service
        /// </summary>
        /// <returns>Response's body</returns>
        Task<string> Execute();
    }
}
