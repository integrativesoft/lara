/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    sealed class FocusDelta : BaseDelta
    {
        [DataMember]
        public string ElementId { get; set; } = string.Empty;

        public FocusDelta() : base(DeltaType.Focus)
        {
        }

        public static void Enqueue(Element element)
        {
            if (element.Document != null)
            {
                element.Document.Enqueue(new FocusDelta
                {
                    ElementId = element.EnsureElementId(),
                });
            }
        }
    }
}
