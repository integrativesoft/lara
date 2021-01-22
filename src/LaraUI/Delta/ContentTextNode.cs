/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class ContentTextNode : ContentNode
    {
        [DataMember]
        public string? Data { get; set; }

        public ContentTextNode() : base(ContentNodeType.Text)
        {            
        }
    }
}
