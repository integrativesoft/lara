/*
Copyright (c) 2019 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    class SubscribeDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; }

        [DataMember]
        public ClientEventSettings Settings { get; set; }

        public SubscribeDelta() : base(DeltaType.Subscribe)
        {
        }
    }

    [DataContract]
    class ClientEventSettings
    {
        [DataMember]
        public string EventName { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool Block { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string BlockElementId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string BlockHTML { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string BlockShownId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ExtraData { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public bool LongRunning { get; set; }

        public static ClientEventSettings CreateFrom(EventSettings settings)
        {
            var client = new ClientEventSettings
            {
                Block = settings.Block,
                EventName = settings.EventName,
                LongRunning = settings.LongRunning
            };
            if (settings.BlockOptions != null)
            {
                client.BlockElementId = settings.BlockOptions.BlockedElementId;
                client.BlockHTML = settings.BlockOptions.ShowHtmlMessage;
                client.BlockShownId = settings.BlockOptions.ShowElementId;
            }
            return client;
        }
    }
}
