/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Globalization;

namespace Integrative.Lara
{
    internal static class TemplateBuilder
    {
        private static readonly string _LibraryUrl;

        static TemplateBuilder()
        {
            _LibraryUrl = ClientLibraryHandler.GetLibraryPath();
        }

        public static void Build(Document document, double keepAliveInterval)
        {
            var head = document.Head;
            var body = document.Body;

            // ensure ids
            head.EnsureElementId();
            body.EnsureElementId();

            // lang
            document.Lang = "en";

            // UTF-8
            var meta = Element.Create("meta");
            meta.SetAttribute("charset", "utf-8");
            head.AppendChild(meta);

            // LaraClient.js
            var script = new HtmlScriptElement
            {
                Src = _LibraryUrl,
                Defer = true
            };
            head.AppendChild(script);

            // initialization script
            var tag = Element.Create("script");
            var id = document.VirtualId.ToString(GlobalConstants.GuidFormat, CultureInfo.InvariantCulture);
            var interval = keepAliveInterval.ToString(CultureInfo.InvariantCulture);
            var code = $"document.addEventListener('DOMContentLoaded', function() {{ LaraUI.initialize('{id}', {interval}); }});";
            tag.AppendChild(new TextNode { Data = code });
            head.AppendChild(tag);
        }
    }
}
