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
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string EventName { get; set; }

        [DataMember]
        public bool Block { get; set; }

        [DataMember]
        public string BlockElementId { get; set; }

        [DataMember]
        public string BlockHTML { get; set; }

        [DataMember]
        public string BlockShownId { get; set; }

        [DataMember]
        public bool LongRunning { get; set; }

        public PlugOptions()
        {
        }

        public PlugOptions(EventSettings settings)
        {
            EventName = settings.EventName;
            Block = settings.Block;
            if (settings.BlockOptions != null)
            {
                BlockElementId = settings.BlockOptions.BlockedElementId;
                BlockHTML = settings.BlockOptions.ShowHtmlMessage;
                BlockShownId = settings.BlockOptions.ShowElementId;
            }
            LongRunning = settings.LongRunning;
        }

        public string ToJSON() => LaraTools.Serialize(this);
        }
}
