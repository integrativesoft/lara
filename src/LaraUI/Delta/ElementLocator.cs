/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
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
        public List<int>? Steps { get; set; }

        public static ElementLocator FromElement(Element element)
        {
            var locator = new ElementLocator
            {
                Steps = new List<int>()
            };
            Build(locator, element);
            return locator;
        }

        private static void Build(ElementLocator locator, Element element)
        {
            if (string.IsNullOrEmpty(element.Id))
            {
                var parent = element.ParentElement;
                if (parent != null)
                {
                    var index = parent.GetChildElementPosition(element);
                    locator.GetSteps().Add(index);
                    Build(locator, parent);
                }
            }
            else
            {
                locator.StartingId = element.Id;
            }
        }
        private List<int> GetSteps()
        {
            return Steps ?? throw new MissingMemberException(nameof(ElementLocator), nameof(Steps));
        }
    }
}
