/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class ClearChildrenDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; } = string.Empty;

        public ClearChildrenDelta() : base(DeltaType.ClearChildren)
        {
        }

        public static void Enqueue(Element element)
        {
            if (element.TryGetQueue(out var document))
            {
                document.Enqueue(new ClearChildrenDelta
                {
                    ElementId = element.Id,
                });
            }
        }
    }
}
