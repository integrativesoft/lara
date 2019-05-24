/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Diagnostics;

namespace Integrative.Lara
{
    public class LaraOptions
    {
        public bool AllowLocalhostOnly { get; set; } = true;
        public bool ShowNotFoundPage { get; set; } = true;
        public int Port { get; set; } = 0;
    }

    public class StartServerOptions : LaraOptions
    {
        public bool ShowExceptions { get; set; }

        public StartServerOptions() : base()
        {
            ShowExceptions = Debugger.IsAttached;
        }
    }
}
