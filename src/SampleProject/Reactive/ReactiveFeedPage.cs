/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;
using Integrative.Lara;
using SampleProject.Main;

namespace SampleProject.Reactive
{
    [LaraPage(Address = PageAddress)]
    internal class ReactiveFeedPage : IPage
    {
        public const string PageAddress = "/reactor3";

        private readonly ReactiveFeedModel _data = new ReactiveFeedModel();

        public Task OnGet()
        {
            var document = LaraUI.Page.Document;
            SampleAppBootstrap.AppendTo(document.Head);
            var builder = new LaraBuilder(document.Body);
            builder.Push("div", "container")
                .Push("div", "form-group form-check")
                    .Push("input", "form-check-input")
                        .GetCurrent(out var checkbox)
                        .Attribute("type", "checkbox")
                        .On("click", () => _data.Checked = checkbox.HasAttribute("checked"))
                    .Pop()
                    .Push("label")
                        .Attribute("for", checkbox.EnsureElementId())
                        .Push("span")
                            .BindInnerText(_data, () => _data.GetCheckedDescription())
                        .Pop()
                    .Pop()
                .Pop()
            .Pop();
            return Task.CompletedTask;
        }
    }

    internal class ReactiveFeedModel : BindableBase
    {
        private bool _checked;

        public bool Checked
        {
            get => _checked;
            set => SetProperty(ref _checked, value);
        }

        public string GetCheckedDescription()
        {
            return Checked ? "Checked" : "Not checked";
        }
    }
}
