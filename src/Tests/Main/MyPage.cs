/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.Main;
using System;
using System.Threading.Tasks;

namespace Integrative.Clara.Tests.Main
{
    class MyPage : BasePage, IDisposable
    {
        public bool Disposed { get; private set; }

        public void Dispose()
        {
            Disposed = true;
        }

        public override Task OnGet(IPageContext context)
        {
            throw new NotImplementedException();
        }
    }
}
