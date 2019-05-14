/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.DOM;
using Microsoft.AspNetCore.Http;

namespace Integrative.Clara.Main
{
    public interface IExecutionContext
    {
        HttpContext Http { get; }
    }

    public interface IPageContext : IExecutionContext
    {
        Document Document { get; }
    }
}
