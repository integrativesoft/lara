/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class SubmitJsDelta : BaseDelta
    {
        [DataMember]
        public string Code { get; set; } = string.Empty;

        [DataMember(EmitDefaultValue = false)]
        public string? Payload { get; set; }

        public SubmitJsDelta() : base(DeltaType.SubmitJs)
        {
        }
    }
}
