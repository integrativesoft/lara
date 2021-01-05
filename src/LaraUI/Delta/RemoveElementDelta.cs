/*
Copyright (c) 2021 Integrative Software LLC
Created: 1/2021
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class RemoveElementDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; } = string.Empty;

        public RemoveElementDelta() : base(DeltaType.RemoveElement)
        {
        }

        public static void Enqueue(Element element)
        {
            if (element.TryGetQueue(out var document))
            {
                document.Enqueue(new RemoveElementDelta
                {
                    ElementId = element.Id
                });
            }
        }
    }
}
