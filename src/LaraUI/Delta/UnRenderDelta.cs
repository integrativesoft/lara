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
    internal class UnRenderDelta : BaseDelta
    {
        [DataMember]
        public NodeLocator? Locator { get; set; }

        public UnRenderDelta() : base(DeltaType.UnRender)
        {
        }

        public static void Enqueue(Document document, IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                document.Enqueue(new UnRenderDelta
                {
                    Locator = NodeLocator.FromNode(node)
                });
            }
        }
    }
}
