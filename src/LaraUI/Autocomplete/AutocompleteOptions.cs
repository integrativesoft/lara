/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    /// <summary>
    /// Autocomplete options
    /// </summary>
    public class AutocompleteOptions
    {
        /// <summary>
        /// Minimum number of characters required to trigger autocomplete suggestions
        /// </summary>
        public int MinLength { get; set; }

        /// <summary>
        /// Automatically focus on selection list
        /// </summary>
        public bool AutoFocus { get; set; } = true;

        /// <summary>
        /// When true, only the suggested values can be selected
        /// </summary>
        public bool Strict { get; set; }

        /// <summary>
        /// Autocomplete suggestions provider
        /// </summary>
        public IAutocompleteProvider? Provider { get; set; }
    }
}
