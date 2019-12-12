/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Lara.Middleware
{
    [DataContract]
    sealed class ClientEventMessage
    {
        [DataMember]
        public List<ElementEventValue>? Values { get; set; }

        [DataMember]
        public string? ExtraData { get; set; }
    }
}
