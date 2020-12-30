/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using System;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    internal class SingleElementPage : IPage
    {
        private Func<Node> ContentFactory { get; }

        public SingleElementPage(Func<Node> contentFactory)
        {
            ContentFactory = contentFactory;
        }

        public Task OnGet()
        {
            var node = ContentFactory();
            LaraUI.Document.Body.AppendChild(node);
            return Task.CompletedTask;
        }
    }
}
