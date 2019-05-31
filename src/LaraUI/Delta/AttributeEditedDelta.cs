/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    sealed class AttributeEditedDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; }

        [DataMember]
        public string Attribute { get; set; }

        [DataMember]
        public string Value { get; set; }

        public AttributeEditedDelta() : base(DeltaType.EditAttribute)
        {
        }

        public static void Enqueue(Element element, string attribute, string value)
        {
            if (element.QueueOpen)
            {
                element.Document.Enqueue(new AttributeEditedDelta
                {
                    Attribute = attribute,
                    ElementId = element.EnsureElementId(),
                    Value = value ?? ""
                });
            }
        }
    }
}
