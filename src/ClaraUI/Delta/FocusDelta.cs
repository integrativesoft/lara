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
    sealed class FocusDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; }

        public FocusDelta() : base(DeltaType.Focus)
        {
        }

        public static void Enqueue(Element element)
        {
            if (element.QueueOpen)
            {
                element.Document.Enqueue(new FocusDelta
                {
                    ElementId = element.EnsureElementId(),
                });
            }
        }
    }
}
