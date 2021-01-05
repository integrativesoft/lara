/*
Copyright (c) 2021 Integrative Software LLC
Created: 1/2021
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal class ContentPlaceholder : ContentNode
    {
        [DataMember]
        public string ElementId { get; set; } = string.Empty;

        public ContentPlaceholder() : base(ContentNodeType.Placeholder)
        {
        }

        public ContentPlaceholder(string id) : this()
        {
            ElementId = id;
        }
    }
}
