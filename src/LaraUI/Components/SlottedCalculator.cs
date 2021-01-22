/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    internal static class SlottedCalculator
    {
        public static void UpdateSlotted(Node node)
        {
            node.IsSlotted = IsParentSlotting(node);
            if (node is WebComponent component)
            {
                var shadow = component.GetShadow();
                UpdateSlotted(shadow);
            }
            if (node is Element element)
            {
                UpdateChildren(element);
            }
        }

        internal static bool IsParentSlotting(Node node)
        {
            var parent = node.ParentElement;
            if (parent == null || parent is Slot)
            {
                return false;
            }
            if (parent is Shadow shadow)
            {
                return shadow.ParentComponent.IsSlotted;
            }
            if (parent is WebComponent component)
            {
                return node is Element element
                       && component.IsSlotActive(element.GetAttributeLower("slot"));
            }
            return parent.IsSlotted;
        }

        private static void UpdateChildren(Element element)
        {
            foreach (var node in element.Children)
            {
                UpdateSlotted(node);
            }
        }
    }
}
