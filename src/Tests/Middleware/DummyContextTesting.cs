/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara.Tests.Middleware
{
    public class DummyContextTesting : IDisposable
    {
        internal readonly DummyContext _context = DummyContext.Create();

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
