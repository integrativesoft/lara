/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Clara.DOM
{
    public class DomEventArgs : EventArgs
    {
        public string EventName { get; }

        internal DomEventArgs(string eventName) : base()
        {
            EventName = eventName;
        }
    }
}
