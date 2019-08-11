/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SampleProject
{
    [LaraPage(Address = PageAddress)]
    class RactiveCollectionPage : IPage
    {
        public const string PageAddress = "/reactor2";

        readonly MyDataTable _data = new MyDataTable();

        public Task OnGet()
        {
            var document = LaraUI.Page.Document;
            SampleAppBootstrap.AppendTo(document.Head);
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
                        .BindChildren(_data.Rows, CreateRowCallback)
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

    class MyDataTable : BindableBase
    {
        public ObservableCollection<MyDataRow> Rows { get; } = new ObservableCollection<MyDataRow>();

        public void AddRow()
        {
            Rows.Add(new MyDataRow());
        }

        public void Remove(MyDataRow row)
        {
            Rows.Remove(row);
        }
    }

    class MyDataRow : BindableBase
    {
        int _counter;

        public int Counter
        {
            get => _counter;
            set => SetProperty(ref _counter, value);
        }

        public void Increase()
        {
            Counter++;
        }
    }
}
