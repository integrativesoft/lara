/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    sealed class KitchenSinkForm : IPage
    {
        public KitchenSinkForm()
        {
        }

        public Task OnGet(IPageContext context)
        {
            BootstrapLoader.AddBootstrap(context.Document.Head);
            context.Document.Body.AppendChild(BuildLayout());
            return Task.CompletedTask;
        }

        private Element BuildLayout()
        {
            var root = Element.Create("form");
            root.Class = "container p-4";
            root.AppendChild(new CounterSample().Build());
            root.AppendChild(new CheckboxSample().Build());
            root.AppendChild(new SelectSample().Build());
            return root;
        }
    }
}
