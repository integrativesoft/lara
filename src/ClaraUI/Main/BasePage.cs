/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;

namespace Integrative.Clara.Main
{
    public abstract class BasePage
    {
        public abstract Task OnGet(IPageContext context);
    }
}
