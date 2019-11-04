/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Middleware;
using System.Globalization;

namespace Integrative.Lara.Main
{
    sealed class TemplateBuilder
    {
        static readonly string _libraryUrl;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline", Justification = "Required behavior")]
        static TemplateBuilder()
        {
            _libraryUrl = ClientLibraryHandler.GetLibraryPath();
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
            var script = new Script
            {
                Src = _libraryUrl,
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
