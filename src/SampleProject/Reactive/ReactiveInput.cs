/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Threading.Tasks;

namespace SampleProject.Reactive
{
    internal class ReactiveInputData : BindableBase
    {
        private string? _text;
        public string? Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        private bool _checked;
        public bool Checked
        {
            get => _checked;
            set => SetProperty(ref _checked, value);
        }
    }

    [LaraPage("/reactiveinput")]
    internal class ReactiveInput : IPage
    {
        private readonly ReactiveInputData _data = new ReactiveInputData();

        public Task OnGet()
        {
            var builder = new LaraBuilder(LaraUI.Document.Body);
            builder.Push("div")
                .Push("input")
                    .Attribute("type", "checkbox")
                    .BindFlagInput("checked", _data, x => x.Checked)
                    .GetCurrent(out var input)
                    .On("input", () => { })
                .Pop()
                .Push("label")
                    .BindInnerText(_data, x => x.Checked ? "checked" : "not checked")
                    .Attribute("for", input.EnsureElementId())
                .Pop()
            .Pop();
            return Task.CompletedTask;
        }
    }
}
