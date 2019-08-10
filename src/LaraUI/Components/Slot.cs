/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Linq;

namespace Integrative.Lara
{
    sealed class Slot : Element
    {
        public string Name { get; set; }

        public Slot(string name) : base("slot")
        {
            Name = name;
        }

        internal override IEnumerable<Node> GetLightSlotted()
        {
            if (ParentElement is Component component)
            {
                return component.GetSlotElements(Name);
            }
            else
            {
                return Enumerable.Repeat(this, 1);
            }
        }
    }
}
