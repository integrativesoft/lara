/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    sealed class SubmitJsDelta : BaseDelta
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Payload { get; set; }

        public SubmitJsDelta() : base(DeltaType.SubmitJS)
        {
        }
    }
}
