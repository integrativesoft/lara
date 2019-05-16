/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Tools;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Clara.Delta
{
    [DataContract]
    sealed class EventResult
    {
        [DataMember]
        public List<BaseDelta> List { get; set; }

        public EventResult()
        {
        }

        public EventResult(List<BaseDelta> list) : this()
        {
            List = list;
        }

        public string ToJSON()
        {
            return ClaraTools.Serialize(this);
        }
    }
}
