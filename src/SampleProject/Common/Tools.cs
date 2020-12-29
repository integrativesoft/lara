/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 9/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System;
using System.Reflection;

namespace SampleProject.Common
{
    internal class Tools
    {
        public static byte[] LoadEmbeddedResource(Assembly assembly, string resourceName)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) throw new InvalidOperationException($"Resource not found: {resourceName}");
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetSpinnerHtml(string message)
        {
            var div = new HtmlDivElement
            {
                Class = "d-flex justify-content-center",
                Children = new Node[]
                {
                    new HtmlDivElement
                    {
                        Class = "spinner-border",
                    },
                    new HtmlDivElement
                    {
                        Class = "ml-2",
                        InnerText = message
                    }
                }
            };
            return div.GetHtml();
        }
    }
}
