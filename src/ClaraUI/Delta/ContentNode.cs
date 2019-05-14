/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Clara.Delta
{
    enum ContentNodeType
    {
        Element = 1,
        Text = 2,
    }

    [DataContract]
    [KnownType(typeof(ContentTextNode))]
    [KnownType(typeof(ContentElementNode))]
    abstract class ContentNode
    {
        [DataMember]
        public ContentNodeType Type { get; set; }

        protected ContentNode(ContentNodeType type)
        {
            Type = type;
        }
    }
}
