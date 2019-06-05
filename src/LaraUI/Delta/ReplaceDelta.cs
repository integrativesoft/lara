/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    sealed class ReplaceDelta : BaseDelta
    {
        [DataMember]
        public string Location { get; set; }

        public ReplaceDelta() : base(DeltaType.Replace)
        {
        }

        public static void Enqueue(Document document, string location)
        {
            document.Enqueue(new ReplaceDelta
            {
                Location = location
            });
        }
    }
}
