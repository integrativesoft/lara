/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Threading.Tasks;

namespace Integrative.Lara.Tests.Main
{
    internal class MyPage : IPage, IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose()
        {
            Disposed = true;
        }

        public Task OnGet()
        {
            throw new NotImplementedException();
        }
    }
}
