/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    [LaraPage(Address = "/")]
    sealed class KitchenSinkForm : IPage
    {
        public KitchenSinkForm()
        {
        }

        public Task OnGet(IPageContext context)
        {
            // Load Bootstrap library.
            BootstrapLoader.AddBootstrap(context.Document.Head);

            // Load document body.
            BuildLayout(context.Document);

            // Done.
            return Task.CompletedTask;
        }

        private void BuildLayout(Document document)
        {
            // Load custom controls in document body.
            var root = Element.Create("form");
            root.Class = "container p-4";
            root.AppendChild(new CounterSample().Build());
            root.AppendChild(new CheckboxSample().Build());
            root.AppendChild(new SelectSample().Build());
            root.AppendChild(new MultiselectSample().Build());
            root.AppendChild(new LockingSample().Build());
            root.AppendChild(new LongRunningSample().Build(document));
            document.Body.AppendChild(root);
        }
    }
}
