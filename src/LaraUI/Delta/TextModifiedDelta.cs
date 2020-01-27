/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class TextModifiedDelta : BaseDelta
    {
        [DataMember]
        public string ParentElementId { get; set; } = string.Empty;

        [DataMember]
        public int ChildNodeIndex { get; set; }

        [DataMember]
        public string? Text { get; set; }

        public TextModifiedDelta() : base(DeltaType.TextModified)
        {
        }

        public static void Enqueue(TextNode node)
        {
            var parent = node.ParentElement;
            if (parent == null || !parent.TryGetQueue(out var document)) return;
            var index = parent.GetChildNodePosition(node);
            document.Enqueue(new TextModifiedDelta
            {
                ParentElementId = parent.EnsureElementId(),
                ChildNodeIndex = index,
                Text = node.Data
            });
        }
    }
}
