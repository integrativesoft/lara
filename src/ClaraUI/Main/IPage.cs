/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;

namespace Integrative.Clara.Main
{
    public interface IPage
    {
        Task OnGet(IPageContext context);
    }
}
