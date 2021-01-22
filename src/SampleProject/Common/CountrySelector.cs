/*
Copyright (c) 2020-2021 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using Integrative.Lara;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SampleProject.Common
{
    internal class CountrySelector : WebComponent, IAutocompleteProvider
    {
        public CountrySelector()
        {
            var auto = new AutocompleteElement
            {
                Class = "form-control"
            };
            var span = new HtmlSpanElement();
            ShadowRoot.Children = new Node[]
            {
                new HtmlDivElement
                {
                    Class = "form-row mt-3",
                    Children = new Node[]
                    {
                        new HtmlDivElement
                        {
                            Class = "form-group col-md-3",
                            Children = new Node[]
                            {
                                new HtmlLabelElement
                                {
                                    InnerText = "Select country"
                                },
                                auto
                            }
                        }
                    }
                },
                new HtmlDivElement
                {
                    Class = "form-row mb-2",
                    Children = new Node[]
                    {
                        new HtmlButtonElement
                        {
                            InnerText = "Show country code",
                            Class = "btn btn-primary"
                        }
                        .Event("click", () => span.InnerText = auto.Value),
                        new HtmlDivElement
                        {
                            Class = "ml-2 pt-2",
                            Children = new Node[] { span }
                        }
                    }
                },
            };
            auto.Start(new AutocompleteOptions
            {
                AutoFocus = true,
                MinLength = 2,
                Strict = true,
                Provider = this
            });
        }

        public Task<AutocompleteResponse> GetAutocompleteList(string term)
        {
            var suggestions = new List<AutocompleteEntry>();
            foreach (var country in CountryList.SearchCountries(term))
            {
                suggestions.Add(new AutocompleteEntry
                {
                    Label = country.Name,
                    Code = country.Code
                });
            }
            var result = new AutocompleteResponse
            {
                Suggestions = suggestions
            };
            return Task.FromResult(result);
        }
    }
}
