/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    sealed class SwapChildrenDelta : BaseDelta
    {
        public SwapChildrenDelta() : base(DeltaType.SwapChildren)
        {            
        }

        [DataMember]
        public string ParentId { get; set; }

        [DataMember]
        public int Index1 { get; set; }
        
        [DataMember]
        public int Index2 { get; set; }

        public static void Enqueue(Element parent, int index1, int index2)
        {
            if (parent.QueueOpen)
            {
                parent.Document.Enqueue(new SwapChildrenDelta
                {
                    ParentId = parent.EnsureElementId(),
                    Index1 = index1,
                    Index2 = index2
                });
            }
        }
    }
}
