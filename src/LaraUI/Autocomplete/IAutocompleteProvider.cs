/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;

namespace Integrative.Lara
{
    /// <summary>
    /// Interface for a class that provides autocomplete suggestions
    /// </summary>
    public interface IAutocompleteProvider
    {
        /// <summary>
        /// Method that provides autocomplete suggestions
        /// </summary>
        /// <param name="term">Search term typed</param>
        /// <returns>Autocomplete response</returns>
        Task<AutocompleteResponse> GetAutocompleteList(string term);
    }
}
