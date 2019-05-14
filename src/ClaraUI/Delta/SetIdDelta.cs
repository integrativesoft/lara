/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.DOM;
using System.Runtime.Serialization;

namespace Integrative.Clara.Delta
{
    [DataContract]
    sealed class SetIdDelta : BaseDelta
    {
        [DataMember]
        public ElementLocator Locator { get; set; }

        [DataMember]
        public string NewId { get; set; }

        public SetIdDelta() : base(DeltaType.SetId)
        {
        }

        public static void Enqueue(Element element)
        {
            if (element.QueueOpen)
            {
                var locator = ElementLocator.FromElement(element);
                element.Document.Enqueue(new SetIdDelta
                {
                    Locator = locator,
                    NewId = element.Id
                });
            }
        }
    }
}
