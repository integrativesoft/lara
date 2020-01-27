/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class ContentAttribute
    {
        [DataMember]
        public string Attribute { get; set; } = string.Empty;

        [DataMember]
        public string Value { get; set; } = string.Empty;
    }
}
