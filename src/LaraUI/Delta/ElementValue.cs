/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class ElementEventValue
    {
        [DataMember]
        public string ElementId { get; set; } = string.Empty;

        [DataMember]
        public string Value { get; set; } = string.Empty;

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
