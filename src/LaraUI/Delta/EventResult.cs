/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tools;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    enum EventResultType
    {
        Success = 0,
        NoSession = 1,
        NoElement = 2
    }

    [DataContract]
    sealed class EventResult
    {
        [DataMember]
        public EventResultType ResultType { get; set; }

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
            return LaraTools.Serialize(this);
        }
    }
}
