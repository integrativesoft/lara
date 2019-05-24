/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Integrative.Lara.Front.Elements
{
    public sealed class Button : Element
    {
        public Button() : base("button")
        {
        }

        public Button(string id) : base("button", id)
        {
        }
    }
}
