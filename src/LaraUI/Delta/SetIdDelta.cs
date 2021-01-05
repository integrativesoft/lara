/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class SetIdDelta : BaseDelta
    {
        [DataMember]
        public string OldId { get; set; } = string.Empty;

        [DataMember]
        public string NewId { get; set; } = string.Empty;

        public SetIdDelta() : base(DeltaType.SetId)
        {
        }

        public static void Enqueue(Element element, string newValue)
        {
            if (element.TryGetQueue(out var document))
            {
                document.Enqueue(new SetIdDelta
                {
                    OldId = element.Id,
                    NewId = newValue
                });
            }
        }
    }
}
