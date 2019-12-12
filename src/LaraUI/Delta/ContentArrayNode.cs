/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    sealed class ContentArrayNode : ContentNode
    {
        [DataMember]
        public List<ContentNode>? Nodes { get; set; }

        public ContentArrayNode() : base(ContentNodeType.Array)
        {
        }
    }
}
