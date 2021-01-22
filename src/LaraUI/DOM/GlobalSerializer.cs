/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System.Globalization;
using System.Threading;

namespace Integrative.Lara
{
    internal static class GlobalSerializer
    {
        private static long _counter;

        public static string GenerateElementId()
        {
            Interlocked.Increment(ref _counter);
            return "_g" + _counter.ToString("X", CultureInfo.InvariantCulture);
        }
    }
}
