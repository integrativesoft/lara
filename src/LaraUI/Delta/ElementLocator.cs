/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class ElementLocator
    {
        [DataMember]
        public string? StartingId { get; set; }

        [DataMember]
        public List<int>? Steps { get; set; } // not in use anymore

        public static ElementLocator FromElement(Element element)
        {
            var locator = new ElementLocator
            {
                Steps = new List<int>(),
                StartingId = element.Id
            };
            return locator;
        }
    }
}
