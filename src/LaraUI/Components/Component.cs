/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Components;
using System;
using System.Collections.Generic;

namespace Integrative.Lara
{
    abstract class Component : Element
    {
        public static void Register(string tagName, Type type)
        {
            ComponentRegistry.Register(tagName, type);
        }

        protected Element Shadow { get; private set; }

        public Component(string tagName) : base(tagName)
        {
        }

        protected void CreateShadow(string tagName)
        {
            Shadow = Create(tagName);
        }

        internal override IEnumerable<Node> GetLightSlotted()
        {
            yield return Shadow ?? this;
        }

        internal IEnumerable<Node> GetSlotElements(string slotName)
        {
            foreach (var node in Children)
            {
                if (node is Element element
                    && element.GetAttributeLower("slot") == slotName)
                {
                    yield return node;
                }                
            }
        }
    }
}
