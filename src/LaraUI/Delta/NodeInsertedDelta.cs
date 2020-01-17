/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class NodeInsertedDelta : BaseDelta
    {
        [DataMember]
        public string ParentElementId { get; set; } = string.Empty;

        [DataMember]
        public int Index { get; set; }

        [DataMember]
        public ContentNode? Node { get; set; }

        public NodeInsertedDelta() : base(DeltaType.Insert)
        {
        }

        public static void Enqueue(Node node, int index)
        {
            var parent = node.ParentElement;
            if (parent != null && parent.TryGetQueue(out var document))
            {
                document.Enqueue(new NodeInsertedDelta
                {
                    ParentElementId = parent.EnsureElementId(),
                    Index = index,
                    Node = node.GetContentNode()
                });
            }
        }
    }
}
