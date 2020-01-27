/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class AttributeRemovedDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; } = string.Empty;

        [DataMember]
        public string Attribute { get; set; } = string.Empty;

        public AttributeRemovedDelta() : base(DeltaType.RemoveAttribute)
        {
        }

        public static void Enqueue(Element element, string attribute)
        {
            if (element.TryGetQueue(out var document))
            {
                document.Enqueue(new AttributeRemovedDelta
                {
                    ElementId = element.EnsureElementId(),
                    Attribute = attribute
                });
            }
        }
    }
}
