/*
Copyright (c) 2019 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    class UnsubscribeDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; }

        [DataMember]
        public string EventName { get; set; }

        public UnsubscribeDelta() : base(DeltaType.Unsubscribe)
        {
        }
    }
}
