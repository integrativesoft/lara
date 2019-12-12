/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    /// <summary>
    /// Default implementation for IAutocompleteEntry
    /// </summary>
    [DataContract]
    public class AutocompleteEntry
    {
        /// <summary>
        /// Optional custom HTML for the autocomplete row shown
        /// </summary>
        /// <returns></returns>
        [DataMember(Name = "html")]
        public string? Html { get; set; }

        /// <summary>
        /// Text to fill the input control when selected
        /// </summary>
        [DataMember(Name = "label")]
        public string? Label { get; set; }

        /// <summary>
        /// Value property associated with the entry
        /// </summary>
        [DataMember(Name = "code")]
        public string? Code { get; set; }

        /// <summary>
        /// Subtitle to show when using default HTML
        /// </summary>
        [DataMember(Name = "subtitle")]
        public string? Subtitle { get; set; }
    }
}
