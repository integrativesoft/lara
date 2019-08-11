/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara.Components
{
    sealed class Shadow : Element
    {
        public const string ShadowTagName = "__shadow";

        public Shadow() : base(ShadowTagName)
        {
        }

        public WebComponent ParentComponent { get; set; }
    }
}
