/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;

namespace Integrative.Lara
{
    public interface INavigation
    {
        void Replace(string location);
        Task FlushPartialChanges();
    }
}
