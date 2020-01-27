/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara.Tests.Middleware
{
    public class DummyContextTesting : IDisposable
    {
        internal readonly DummyContext Context = DummyContext.Create();

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}
