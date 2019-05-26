/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.DOM;
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

        readonly Document _document;

        private TemplateBuilder(Document document)
        {
            _document = document;
        }

        public static void Build(Document document)
        {
            var builder = new TemplateBuilder(document);
            builder.BuildInternal();
        }

        private void BuildInternal()
        {
            var head = _document.Head;
            var body = _document.Body;

            // ensure ids
            head.EnsureElementId();
            body.EnsureElementId();

            // lang
            _document.Lang = "en";

            // UTF-8
            var meta = Element.Create("meta");
            meta.SetAttribute("charset", "utf-8");
            head.AppendChild(meta);

            // LaraUI.js
            var script = Element.Create("script");
            script.SetAttribute("src", _libraryUrl);
            script.SetAttribute("defer", null);
            head.AppendChild(script);

            // initialization script
            script = Element.Create("script");
            string value = _document.VirtualId.ToString(GlobalConstants.GuidFormat);
            string code = $"document.addEventListener('DOMContentLoaded', function() {{ LaraUI.initialize('{value}'); }});";
            script.AppendChild(new TextNode { Data = code });
            head.AppendChild(script);
        }
    }
}
