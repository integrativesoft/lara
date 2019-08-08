/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SampleProject
{
    [LaraPage(Address = "/reactor1")]
    class SimpleReactivePage : IPage
    {
        readonly SimpleData _data = new SimpleData();

        public Task OnGet()
        {
            var builder = new LaraBuilder(LaraUI.Page.Document.Body);
            builder.Push("div")
                .Push("span")
                    .BindInnerText(_data, x => x.Counter.ToString())
                .Pop()
                .Push("button")
                    .On(new EventSettings
                    {
                        Block = true,
                        Handler = () =>
                        {
                            _data.IncreaseCounter();
                            return Task.CompletedTask;
                        }
                    })
                .Pop()
            .Pop();
            return Task.CompletedTask;
        }
    }

    class SimpleData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int Counter { get; private set; }

        public void IncreaseCounter()
        {
            Counter++;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Counter)));
        }
    }
}
