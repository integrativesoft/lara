/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Interface for web pages
    /// </summary>
    public interface IPage
    {
        /// <summary>
        /// Called when replying to the initial HTTP GET request.
        /// </summary>
        /// <param name="context">The execution context.</param>
        Task OnGet(IPageContext context);
    }
}
