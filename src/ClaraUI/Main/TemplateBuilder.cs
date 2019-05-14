/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Clara.DOM;
using Integrative.Clara.Middleware;
using System;

namespace Integrative.Clara.Main
{
    sealed class TemplateBuilder
    {
        readonly static string _libraryUrl;

        static TemplateBuilder()
        {
            _libraryUrl = ClientLibraryHandler.GetLibraryPath();
        }

        readonly Document _document;
        readonly Guid _guid;

        public TemplateBuilder(Document document, Guid guid)
        {
            _document = document;
            _guid = guid;
        }

        public void Build()
        {
            var head = _document.Head;
            var body = _document.Body;

            // ensure ids
            head.EnsureElementId();
            body.EnsureElementId();

            // lang
            _document.Lang = "en";

            // UTF-8
            var meta = new Element("meta");
            meta.SetAttribute("charset", "utf-8");
            head.AppendChild(meta);

            // clara.js
            var script = new Element("script");
            script.SetAttribute("src", _libraryUrl);
            script.SetAttribute("defer", null);
            head.AppendChild(script);

            // initialization script
            script = new Element("script");
            string value = _guid.ToString(GlobalConstants.GuidFormat);
            string code = $"document.addEventListener('DOMContentLoaded', function() {{ ClaraUI.initialize('{value}'); }});";
            script.AppendChild(new TextNode { Data = code });
            head.AppendChild(script);
        }
    }
}
