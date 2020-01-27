/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using Integrative.Lara;

namespace SampleProject.Components
{
    [LaraWebComponent(MyCard)]
    // ReSharper disable once UnusedType.Global
    internal class CardComponent : WebComponent
    {
        private const string MyCard = "my-card";

        private readonly Element _div, _spanSubtitle;

        public CardComponent() : base(MyCard)
        {
            _div = Create("div");
            var spanTitle = Create("span");
            _spanSubtitle = Create("span");
            var builder = new LaraBuilder(ShadowRoot);
            builder.Push(_div, "card")
                .Push("div", "card-body")
                    .Push("h5", "card-title")
                        .AddNode(spanTitle)
                    .Pop()
                    .Push("h6", "card-subtitle")
                        .AddNode(_spanSubtitle)
                    .Pop()
                    .Push("p", "card-text")
                        .Push("slot")
                        .Pop()
                    .Pop()
                .Pop()
            .Pop();
        }

        private string? _title;

        public string? Heading
        {
            get => _title;
            set
            {
                _title = value;
                _spanSubtitle.InnerText = value;
            }
        }

        private string? _subtitle;

        public string? Subtitle
        {
            get => _subtitle;
            set
            {
                _subtitle = value;
                _spanSubtitle.InnerText = value;
            }
        }

        protected override IEnumerable<string> GetObservedAttributes()
        {
            return new[] { "class", "heading", "subtitle" };
        }

        protected override void OnAttributeChanged(string attribute)
        {
            if (attribute == "class")
            {
                _div.Class = GetAttribute("class");
                _div.AddClass("card");
            }
            else if (attribute == "heading")
            {
                Heading = GetAttribute("heading");
            }
            else if (attribute == "subtitle")
            {
                Subtitle = GetAttribute("subtitle");
            }
        }
    }
}
