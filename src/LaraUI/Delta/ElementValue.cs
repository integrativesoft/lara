/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    sealed class ElementValue
    {
        [DataMember]
        public string ElementId { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}
