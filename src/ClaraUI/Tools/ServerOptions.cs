/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Clara.Tools
{
    public sealed class ServerOptions
    {
        public bool AllowLocalhostOnly { get; set; } = true;
        public bool ShowNotFoundPage { get; set; } = true;
        public int Port { get; set; } = 0;
    }
}
