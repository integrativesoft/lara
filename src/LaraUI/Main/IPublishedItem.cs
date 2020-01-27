/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    internal interface IPublishedItem
    {
        Task Run(Application app, HttpContext http, LaraOptions options);
    }
}
