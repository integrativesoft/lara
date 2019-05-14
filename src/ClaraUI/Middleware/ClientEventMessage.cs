/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Delta;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Clara.Middleware
{
    [DataContract]
    sealed class ClientEventMessage
    {
        [DataMember]
        public List<ElementValue> Values { get; set; }
    }
}
