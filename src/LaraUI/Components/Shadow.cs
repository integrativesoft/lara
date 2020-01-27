/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Linq;

namespace Integrative.Lara
{
    internal sealed class Shadow : Element
    {
        private const string ShadowTagName = "__shadow";

        public Shadow(WebComponent parent) : base(ShadowTagName)
        {
            ParentComponent = parent;
        }

        public WebComponent ParentComponent { get; }

        internal override IEnumerable<Node> GetLightSlotted()
        {
            return Enumerable.Empty<Node>();
        }

        internal override bool IsPrintable => false;
    }
}
