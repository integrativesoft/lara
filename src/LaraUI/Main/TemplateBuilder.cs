/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Middleware;

namespace Integrative.Lara.Main
{
    sealed class TemplateBuilder
    {
        static readonly string _libraryUrl;

        static TemplateBuilder()
        {
            _libraryUrl = ClientLibraryHandler.GetLibraryPath();
        }

        public static void Build(Document document, LaraOptions options)
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

            // jQuery.js
            var script = new Script
            {
                Src = options.AddressJQuery,
                Defer = true
            };
            head.AppendChild(script);

            // LaraClient.js
            script = new Script
            {
                Src = _libraryUrl,
                Defer = true
            };
            head.AppendChild(script);

            // initialization script
            var tag = Element.Create("script");
            string value = document.VirtualId.ToString(GlobalConstants.GuidFormat);
            string code = $"document.addEventListener('DOMContentLoaded', function() {{ LaraUI.initialize('{value}'); }});";
            tag.AppendChild(new TextNode { Data = code });
            head.AppendChild(tag);
        }
    }
}
