/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara.Tests.Middleware
{
    static class PublishHelper
    {
        readonly static object _mylock = new object();
        static bool _published;

        public static void PublishIfNeeded()
        {
            lock (_mylock)
            {
                if (!_published)
                {
                    _published = true;
                    LaraUI.PublishAssemblies();
                }
            }
        }

        /*public static void RunInsideLock(Action action)
        {
            lock (_mylock)
            {
                action();
            }
        }*/
    }
}
