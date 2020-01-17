/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class SwapChildrenDelta : BaseDelta
    {
        public SwapChildrenDelta() : base(DeltaType.SwapChildren)
        {            
        }

        [DataMember]
        public string ParentId { get; set; } = string.Empty;

        [DataMember]
        public int Index1 { get; set; }
        
        [DataMember]
        public int Index2 { get; set; }

        public static void Enqueue(Element parent, int index1, int index2)
        {
            if (parent.TryGetQueue(out var document))
            {
                document.Enqueue(new SwapChildrenDelta
                {
                    ParentId = parent.EnsureElementId(),
                    Index1 = index1,
                    Index2 = index2
                });
            }
        }
    }
}
