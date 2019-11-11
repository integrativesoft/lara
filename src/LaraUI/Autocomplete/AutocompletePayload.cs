/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Autocomplete
{
    [DataContract]
    class AutocompletePayload
    {
        [DataMember]
        public string ElementId { get; set; }

        [DataMember]
        public bool AutoFocus { get; set; }

        [DataMember]
        public int MinLength { get; set; }

        [DataMember]
        public bool Strict { get; set; }
    }
}
