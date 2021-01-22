/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class NodeAddedDelta : BaseDelta
    {
        [DataMember]
        public string ParentId { get; set; } = string.Empty;

        [DataMember]
        public ContentNode? Node { get; set; }

        public NodeAddedDelta() : base(DeltaType.Append)
        {
        }

        public static void Enqueue(Node node)
        {
            var parent = node.ParentElement;
            if (parent == null || !parent.TryGetQueue(out var document)) return;
            var parentId = parent.Id;
            var content = node.GetContentNode();
            document.Enqueue(new NodeAddedDelta
            {
                ParentId = parentId,
                Node = content,
            });
        }
    }
}
