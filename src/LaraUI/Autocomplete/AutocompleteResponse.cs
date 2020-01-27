/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Lara
{
    /// <summary>
    /// Autocomplete results
    /// </summary>
    [DataContract]
    public class AutocompleteResponse
    {
        /// <summary>
        /// List of autocomplete entries
        /// </summary>
        [DataMember]
        public List<AutocompleteEntry>? Suggestions { get; set; }
    }
}
