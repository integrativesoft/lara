/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Clara.Delta
{
    [DataContract]
    sealed class ContentTextNode : ContentNode
    {
        [DataMember]
        public string Data { get; set; }

        public ContentTextNode() : base(ContentNodeType.Text)
        {            
        }
    }
}
