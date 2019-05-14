/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.DOM;
using System.Runtime.Serialization;

namespace Integrative.Clara.Delta
{
    [DataContract]
    sealed class AttributeRemovedDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; }

        [DataMember]
        public string Attribute { get; set; }

        public AttributeRemovedDelta() : base(DeltaType.RemoveAttribute)
        {
        }

        public static void Enqueue(Element element, string attribute)
        {
            if (element.QueueOpen)
            {
                element.Document.Enqueue(new AttributeRemovedDelta
                {
                    ElementId = element.EnsureElementId(),
                    Attribute = attribute
                });
            }
        }
    }
}
