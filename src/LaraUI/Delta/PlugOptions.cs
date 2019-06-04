/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tools;
using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    sealed class PlugOptions
    {
        [DataMember]
        public string EventName { get; set; }

        [DataMember]
        public bool Block { get; set; }

        [DataMember]
        public string BlockElementId { get; set; }

        [DataMember]
        public string BlockHTML { get; set; }

        public PlugOptions()
        {
        }

        public PlugOptions(EventSettings settings)
        {
            EventName = settings.EventName;
            Block = settings.Block;
            BlockElementId = settings.BlockElementId;
            BlockHTML = settings.BlockHtmlMessage;
        }

        public string ToJSON() => LaraTools.Serialize(this);
        }
}
