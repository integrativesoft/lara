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
        public string ElementId { get; set; } = string.Empty;

        [DataMember]
        public string Attribute { get; set; } = string.Empty;

        [DataMember]
        public string Value { get; set; } = string.Empty;

        public AttributeEditedDelta() : base(DeltaType.EditAttribute)
        {
        }

        public static void Enqueue(Element element, string attribute, string? value)
        {
            if (element.TryGetQueue(out var document))
            {
                document.Enqueue(new AttributeEditedDelta
                {
                    Attribute = attribute,
                    ElementId = element.EnsureElementId(),
                    Value = value ?? ""
                });
            }
        }
    }
}
