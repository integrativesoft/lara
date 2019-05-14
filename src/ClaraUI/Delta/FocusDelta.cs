/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Clara.Delta
{
    [DataContract]
    sealed class FocusDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; }

        public FocusDelta() : base(DeltaType.Focus)
        {
        }        
    }
}
