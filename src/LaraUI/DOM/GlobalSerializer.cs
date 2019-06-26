/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System.Threading;

namespace Integrative.Lara.DOM
{
    static class GlobalSerializer
    {
        static long _counter = 0;

        public static string GenerateElementId()
        {
            Interlocked.Increment(ref _counter);
            return "_g" + _counter.ToString();
        }
    }
}
