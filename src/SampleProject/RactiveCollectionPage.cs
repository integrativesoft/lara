/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SampleProject
{
    [LaraPage(Address = "/reactor2")]
    class RactiveCollectionPage : IPage
    {
        readonly MyDataTable _data = new MyDataTable();

        public Task OnGet()
        {
            var document = LaraUI.Page.Document;
            BootstrapLoader.AddBootstrap(document.Head);
            var builder = new LaraBuilder(document.Body);
            builder.Push("div", "p-2")
                .Push("button", "btn btn-primary")
                    .AddTextNode("add row")
                    .On("click", () => _data.AddRow())
                .Pop()
            .Pop()
            .Push("div", "p-2")
                .Push("table")
                    .Push("tbody")
                        .BindChildren(new BindChildrenOptions<MyDataRow>
                        {
                            Collection = _data.Rows,
                            CreateCallback = CreateRowCallback,
                        })
                    .Pop()
                .Pop()
            .Pop();
            return Task.CompletedTask;
        }

        private Element CreateRowCallback(MyDataRow dataRow)
        {
            var row = Element.Create("tr");
            var builder = new LaraBuilder(row);
            builder.Push("td")
                .Push("span")
                    .BindInnerText(dataRow, x => x.Counter.ToString())
                .Pop()
            .Pop()
            .Push("td")
                .Push("button", "btn btn-primary ml-3 mr-2 mb-1")
                    .AddTextNode("increase")
                    .On("click", () => dataRow.Increase())
                .Pop()
            .Pop()
            .Push("td")
                .Push("button", "btn btn-secondary")
                    .AddTextNode("remove")
                    .On("click", () => _data.Remove(dataRow))
                .Pop()
            .Pop();
            return row;
        }
    }

    class MyDataTable : INotifyPropertyChanged
    {
        public int Counter { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<MyDataRow> Rows { get; } = new ObservableCollection<MyDataRow>();

        public void AddRow()
        {
            IncreaseCounter();
            Rows.Add(new MyDataRow(Counter));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rows)));
        }

        public void Remove(MyDataRow row)
        {
            Rows.Remove(row);
        }

        private void IncreaseCounter()
        {
            Counter++;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Counter)));
        }
    }

    class MyDataRow : INotifyPropertyChanged
    {
        public int Counter { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MyDataRow(int startValue)
        {
            Counter = startValue;
        }

        public void Increase()
        {
            Counter++;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Counter)));
        }
    }
}
