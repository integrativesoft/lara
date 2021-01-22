/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class NodeLocator
    {
        [DataMember]
        public string StartingId { get; set; } = string.Empty;

        [DataMember]
        public int? ChildIndex { get; set; }

        public static NodeLocator FromNode(Node node)
        {
            if (node is Element element)
            {
                return new NodeLocator
                {
                    StartingId = element.Id
                };
            }
            var parent = node.ParentElement;
            if (parent == null)
            {
                throw new ArgumentException("NodeLocator from orphan non-element node");
            }
            return new NodeLocator
            {
                StartingId = parent.Id,
                ChildIndex = parent.GetChildNodePosition(node)
            };
        }
    }
}
