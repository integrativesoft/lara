/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
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
            if (node.ParentElement.QueueOpen)
            {
                var parentId = node.ParentElement.EnsureElementId();
                var content = node.GetContentNode();
                node.Document.Enqueue(new NodeAddedDelta
                {
                    ParentId = parentId,
                    Node = content,
                });
            }
        }
    }
}
