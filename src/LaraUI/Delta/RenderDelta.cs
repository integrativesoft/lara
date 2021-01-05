/*
Copyright (c) 2021 Integrative Software LLC
Created: 1/2021
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal class RenderDelta : BaseDelta
    {
        [DataMember]
        public NodeLocator? Locator { get; set; }

        [DataMember]
        public ContentNode? Node { get; set; }

        public RenderDelta() : base(DeltaType.Render)
        {
        }

        public static void Enqueue(Document document, IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                document.Enqueue(new RenderDelta
                {
                    Locator = NodeLocator.FromNode(node),
                    Node = node.GetContentNode()
                });
            }
        }
    }
}
