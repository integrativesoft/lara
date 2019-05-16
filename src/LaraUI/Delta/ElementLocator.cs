/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.DOM;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    sealed class ElementLocator
    {
        [DataMember]
        public string StartingId { get; set; }

        [DataMember]
        public List<int> Steps { get; set; }

        public static ElementLocator FromElement(Element element)
        {
            var locator = new ElementLocator
            {
                Steps = new List<int>()
            };
            Build(locator, element);
            return locator;
        }

        /*private static void BuildIgnoringId(ElementLocator locator, Element element)
        {
            var parent = element.ParentElement;
            int index = parent.GetChildPosition(element);
            locator.Steps.Add(index);
            Build(locator, parent);
        }*/

        private static void Build(ElementLocator locator, Element element)
        {
            if (string.IsNullOrEmpty(element.Id))
            {
                var parent = element.ParentElement;
                if (parent != null)
                {
                    int index = parent.GetChildPosition(element);
                    locator.Steps.Add(index);
                    Build(locator, parent);
                }
            }
            else
            {
                locator.StartingId = element.Id;
            }
        }
    }
}
