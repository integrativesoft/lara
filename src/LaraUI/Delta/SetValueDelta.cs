/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class SetValueDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; } = string.Empty;

        [DataMember]
        public string? Value { get; set; }

        public SetValueDelta() : base(DeltaType.SetValue)
        {
        }

        public static void Enqueue(Element element, string? value)
        {
            if (element.TryGetQueue(out var document))
            {
                document.Enqueue(new SetValueDelta
                {
                    ElementId = element.EnsureElementId(),
                    Value = value
                });
            }
        }
    }
}
