/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.DOM;
using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    sealed class SetValueDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; }

        [DataMember]
        public string Value { get; set; }

        public SetValueDelta() : base(DeltaType.SetValue)
        {
        }

        public static void Enqueue(Element element, string value)
        {
            if (element.QueueOpen)
            {
                element.Document.Enqueue(new SetValueDelta
                {
                    ElementId = element.EnsureElementId(),
                    Value = value
                });
            }
        }
    }
}
