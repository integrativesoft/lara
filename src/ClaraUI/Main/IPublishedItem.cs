/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Integrative.Clara.Main
{
    interface IPublishedItem
    {
        Task Run(HttpContext http);
    }
}
