/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    internal enum ContentNodeType
    {
        Element = 1,
        Text = 2,
        Array = 3
    }

    [DataContract]
    [KnownType(typeof(ContentTextNode))]
    [KnownType(typeof(ContentElementNode))]
    [KnownType(typeof(ContentArrayNode))]
    internal abstract class ContentNode
    {
        [DataMember]
        public ContentNodeType Type { get; set; }

        protected ContentNode(ContentNodeType type)
        {
            Type = type;
        }
    }
}
