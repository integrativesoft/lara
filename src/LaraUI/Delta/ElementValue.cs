/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    sealed class ElementEventValue
    {
        [DataMember]
        public string ElementId { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public bool Checked { get; set; }

        public override string ToString()
        {
            return $"#{ElementId}='{Value}'{GetCheckedSuffix()}";
        }

        private string GetCheckedSuffix()
        {
            if (Checked)
            {
                return " checked";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
