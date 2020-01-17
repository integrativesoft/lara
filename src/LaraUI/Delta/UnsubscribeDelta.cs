/*
Copyright (c) 2019 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal class UnsubscribeDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; } = string.Empty;

        [DataMember]
        public string EventName { get; set; } = string.Empty;

        public UnsubscribeDelta() : base(DeltaType.Unsubscribe)
        {
        }
    }
}
