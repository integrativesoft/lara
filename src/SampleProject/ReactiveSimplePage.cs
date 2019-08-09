/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject
{
    [LaraPage(Address = "/reactor1")]
    class ReactiveSimplePage : IPage
    {
        readonly SimpleData _data = new SimpleData();

        public Task OnGet()
        {
            var document = LaraUI.Page.Document;
            BootstrapLoader.AddBootstrap(document.Head);
            var builder = new LaraBuilder(document.Body);
            builder.Push("div", "p-2")
                .Push("span")
                    .BindInnerText(_data, x => x.Counter.ToString())
                .Pop()
            .Pop()
            .Push("div", "p-2")
                .Push("button", "btn btn-primary")
                    .On("click", () => _data.IncreaseCounter())
                    .AddTextNode("increase")
                .Pop()
            .Pop();
            return Task.CompletedTask;
        }
    }

    class SimpleData : BindableBase
    {
        int _counter;

        public int Counter
        {
            get => _counter;
            set => SetProperty(ref _counter, value);
        }

        public void IncreaseCounter()
        {
            Counter++;
        }
    }
}
