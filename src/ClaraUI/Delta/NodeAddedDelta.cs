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
    sealed class NodeAddedDelta : BaseDelta
    {
        [DataMember]
        public string ParentId { get; set; }

        [DataMember]
        public ContentNode Node { get; set; }

        public NodeAddedDelta() : base(DeltaType.Append)
        {
        }

        public static void Enqueue(Node node)
        {
            if (node.QueueOpen)
            {
                node.Document.Enqueue(new NodeAddedDelta
                {
                    ParentId = node.ParentElement.EnsureElementId(),
                    Node = node.GetContentNode(),
                });
            }
        }
    }
}
