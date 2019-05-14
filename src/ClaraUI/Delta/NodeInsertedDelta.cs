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
    sealed class NodeInsertedDelta : BaseDelta
    {
        [DataMember]
        public string ParentElementId { get; set; }

        [DataMember]
        public int Index { get; set; }

        [DataMember]
        public ContentNode Node { get; set; }

        public NodeInsertedDelta() : base(DeltaType.Insert)
        {
        }

        public static void Enqueue(Node node, int index)
        {
            if (node.QueueOpen)
            {
                node.Document.Enqueue(new NodeInsertedDelta
                {
                    ParentElementId = node.ParentElement.EnsureElementId(),
                    Index = index,
                    Node = node.GetContentNode()
                });
            }
        }
    }
}
