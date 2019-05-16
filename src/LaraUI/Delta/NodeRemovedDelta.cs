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
    sealed class NodeRemovedDelta : BaseDelta
    {
        [DataMember]
        public string ParentId { get; set; }

        [DataMember]
        public int ChildIndex { get; set; }

        public NodeRemovedDelta() : base(DeltaType.Remove)
        {
        }

        public static void Enqueue(Element parent, int index)
        {
            if (parent.QueueOpen)
            {
                parent.Document.Enqueue(new NodeRemovedDelta
                {
                    ParentId = parent.EnsureElementId(),
                    ChildIndex = index,
                });
            }
        }
    }
}
