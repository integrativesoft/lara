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
    sealed class TextModifiedDelta : BaseDelta
    {
        [DataMember]
        public string ParentElementId { get; set; }

        [DataMember]
        public int ChildNodeIndex { get; set; }

        [DataMember]
        public string Text { get; set; }

        public TextModifiedDelta() : base(DeltaType.TextModified)
        {
        }

        public static void Enqueue(TextNode node)
        {
            if (node.QueueOpen)
            {
                int index = node.ParentElement.GetChildPosition(node);
                node.Document.Enqueue(new TextModifiedDelta
                {
                    ParentElementId = node.ParentElement.EnsureElementId(),
                    ChildNodeIndex = index,
                    Text = node.Data
                });
            }
        }
    }
}
