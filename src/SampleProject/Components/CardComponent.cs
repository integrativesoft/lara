/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Collections.Generic;

namespace SampleProject
{
    [LaraWebComponent(MyCard)]
    class CardComponent : WebComponent
    {
        public const string MyCard = "my-card";

        readonly Element _div, _spanTitle, _spanSubtitle;

        public CardComponent() : base(MyCard)
        {
            _div = Create("div");
            _spanTitle = Create("span");
            _spanSubtitle = Create("span");
            var builder = new LaraBuilder(ShadowRoot);
            builder.Push(_div, "card")
                .Push("div", "card-body")
                    .Push("h5", "card-title")
                        .AddNode(_spanTitle)
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

        string _title;

        public string Heading
        {
            get => _title;
            set
            {
                _title = value;
                _spanTitle.SetInnerText(value);
            }
        }

        string _subtitle;

        public string Subtitle
        {
            get => _subtitle;
            set
            {
                _subtitle = value;
                _spanSubtitle.SetInnerText(value);
            }
        }

        protected override IEnumerable<string> GetObservedAttributes()
        {
            return new string[] { "class", "heading", "subtitle" };
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
