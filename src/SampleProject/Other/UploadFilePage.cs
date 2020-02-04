/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleProject.Other
{
    [LaraPage(Address)]
    internal class UploadFilePage : IPage
    {
        public const string Address = "/upload";

        private readonly InputElement _file = new InputElement();
        private readonly Element _span = Element.Create("span");

        public Task OnGet()
        {
            var builder = new LaraBuilder(LaraUI.Document.Body);
            builder.Push("div")
                .Push(_file)
                    .Attribute("type", "file")
                    .FlagAttribute("multiple", true)
                .Pop()
            .Pop()
            .Push("div")
                .Push("button")
                    .InnerText("Upload via Ajax")
                    .On(new EventSettings
                    {
                        EventName = "click",
                        UploadFiles = true,
                        Handler = ClickHandler
                    })
                .Pop()
            .Pop()
            .Push("div")
                .Push("button")
                    .InnerText("Upload via WebSocket")
                    .On(new EventSettings
                    {
                        EventName = "click",
                        UploadFiles = true,
                        Handler = ClickHandler,
                        LongRunning = true
                    })
                .Pop()
            .Pop()
            .Push("div")
                .AddNode(_span)
            .Pop();
            return Task.CompletedTask;
        }

        private Task ClickHandler()
        {
            _span.InnerText = GetUploadText();
            return Task.CompletedTask;
        }

        private string GetUploadText()
        {
            if (_file.Files.Count == 0)
            {
                return "No files uploaded";
            }

            return "Uploaded: " + string.Join(", ", GetFileNames());
        }

        private IEnumerable<string> GetFileNames()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var file in _file.Files)
            {
                var text = $"{file.FileName} ({file.Length} bytes)";
                yield return text;
            }
        }
    }
}
