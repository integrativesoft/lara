/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    public sealed class EventSettings
    {
        public string EventName { get; set; }
        public bool Block { get; set; }
        public string BlockElementId { get; set; }
        public string BlockHtmlMessage { get; set; }
        public Func<IPageContext, Task> Handler { get; set; }
    }
}
