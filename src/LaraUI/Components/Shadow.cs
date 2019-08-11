/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Linq;

namespace Integrative.Lara.Components
{
    sealed class Shadow : Element
    {
        public const string ShadowTagName = "__shadow";

        public Shadow() : base(ShadowTagName)
        {
        }

        public WebComponent ParentComponent { get; set; }

        internal override IEnumerable<Node> GetLightSlotted()
        {
            return Enumerable.Empty<Node>();
        }
    }
}
