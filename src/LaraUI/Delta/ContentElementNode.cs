/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    sealed class ContentElementNode : ContentNode
    {
        [DataMember]
        public string TagName { get; set; }

        [DataMember]
        public List<ContentAttribute> Attributes { get; set; }

        [DataMember]
        public List<ContentNode> Children { get; set; }

        public ContentElementNode() : base(ContentNodeType.Element)
        {
        }
    }
}
