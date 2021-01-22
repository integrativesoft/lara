/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class PlugOptions
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool Block { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string? BlockElementId { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        // ReSharper disable once InconsistentNaming
        public string? BlockHTML { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string? BlockShownId { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool LongRunning { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool UploadFiles { get; set; }

        public PlugOptions()
        {
        }

        public PlugOptions(EventSettings settings)
        {
            Block = settings.Block;
            if (settings.BlockOptions != null)
            {
                BlockElementId = settings.BlockOptions.BlockedElementId;
                BlockHTML = settings.BlockOptions.ShowHtmlMessage;
                BlockShownId = settings.BlockOptions.ShowElementId;
            }
            LongRunning = settings.LongRunning;
            UploadFiles = settings.UploadFiles;
        }

        // ReSharper disable once InconsistentNaming
        public string ToJSON() => LaraTools.Serialize(this);
    }
}
