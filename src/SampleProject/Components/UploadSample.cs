/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleProject.Components
{
    internal class UploadSample : WebComponent
    {
        public UploadSample()
        {
            var file = new HtmlInputElement
            {
                Type = "file",
                Multiple = true,
                Style = "margin: 5px"
            };
            var span = new HtmlSpanElement();
            ShadowRoot.Children = new Node[]
            {
                new HtmlDivElement
                {
                    Children = new Node[]
                    {
                        file
                    }
                },
                new HtmlDivElement
                {
                    Children = new Node[]
                    {
                        new HtmlButtonElement
                        {
                            InnerText = "Upload via Ajax",
                            Style = "margin: 5px"
                        }
                        .Event(new EventSettings
                        {
                            EventName = "click",
                            UploadFiles = true,
                            Handler = () => {
                                span.InnerText = GetUploadText(file);
                                return Task.CompletedTask;
                                },
                        }),
                        new HtmlButtonElement
                        {
                            InnerText = "Upload via Websocket",
                            Style = "margin: 5px"
                        }
                        .Event(new EventSettings
                        {
                            EventName = "click",
                            UploadFiles = true,
                            Handler = () => {
                                span.InnerText = GetUploadText(file);
                                return Task.CompletedTask;
                                },
                            LongRunning = true
                        }),
                    }
                },
                new HtmlDivElement
                {
                    Style = "margin: 5px",
                    Children = new Node[] { span }
                }
            };
        }

        private static string GetUploadText(HtmlInputElement input)
        {
            if (input.Files.Count == 0)
            {
                return "No files uploaded";
            }

            return "Uploaded: " + string.Join(", ", GetFileNames(input));
        }

        private static IEnumerable<string> GetFileNames(HtmlInputElement input)
        {
            foreach (var file in input.Files)
            {
                var text = $"{file.FileName} ({file.Length} bytes)";
                yield return text;
            }
        }
    }
}
