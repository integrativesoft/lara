/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Lara
{
    internal enum EventResultType
    {
        Success = 0,
        NoSession = 1,
        NoElement = 2,
        OutOfSequence = 3
    }

    [DataContract]
    internal sealed class EventResult
    {
        [DataMember]
        public EventResultType ResultType { get; set; }

        [DataMember]
        public List<BaseDelta>? List { get; set; }

        public EventResult()
        {
        }

        public EventResult(List<BaseDelta> list) : this()
        {
            List = list;
        }

        // ReSharper disable once InconsistentNaming
        public string ToJSON()
        {
            return LaraTools.Serialize(this);
        }
    }
}
