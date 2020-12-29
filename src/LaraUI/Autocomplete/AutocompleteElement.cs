/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System;

namespace Integrative.Lara
{
    /// <summary>
    /// Autocomplete web component
    /// </summary>
    public class AutocompleteElement : WebComponent
    {
        /// <summary>
        /// Autocomplete's custom HTML tag
        /// </summary>
        public const string CustomTag = "lara-autocomplete";

        /// <summary>
        /// Returns the inner input element
        /// </summary>
        public HtmlInputElement InnerInput { get; } = new HtmlInputElement();

        /// <summary>
        /// Constructor
        /// </summary>
        public AutocompleteElement() : base(CustomTag)
        {
            InnerInput.Autocomplete = "off";
            ShadowRoot.AppendChild(InnerInput);
        }

        /// <summary>
        /// Input element's class
        /// </summary>
        public override string? Class
        {
            get => InnerInput.Class;
            set => InnerInput.Class = value;
        }

        private bool _pending, _applied;

        private AutocompleteOptions? _options;

        internal AutocompleteOptions? GetOptions() => _options;

        /// <summary>
        /// Enables the autocomplete functionality
        /// </summary>
        /// <param name="options">Autocomplete options</param>
        public void Start(AutocompleteOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (Document == null)
            {
                _pending = true;
            }
            else
            {
                DestroyAutocomplete();
                SubmitAutocomplete(Document, options);
            }
        }

        /// <summary>
        /// Stops the autocomplete functionality
        /// </summary>
        public void Stop()
        {
            DestroyAutocomplete();
        }

        /// <summary>
        /// Establishes autocomplete if pending
        /// </summary>
        protected override void OnConnect()
        {
            if (!_pending) return;
            _pending = false;
            DestroyAutocomplete();
            if (Document != null && _options != null)
            {
                SubmitAutocomplete(Document, _options);
            }
        }

        /// <summary>
        /// Disposes autocomplete references on browser
        /// </summary>
        protected override void OnDisconnect()
        {
            DestroyAutocomplete();
        }

        internal string AutocompleteId { get; private set; } = string.Empty;

        private void SubmitAutocomplete(Document document, AutocompleteOptions options)
        {
            AutocompleteId = GetAutocompleteKey(document);
            AutocompleteService.Register(AutocompleteId, this);
            _applied = true;
            _pending = false;
            var payload = new AutocompletePayload
            {
                AutoFocus = options.AutoFocus,
                ElementId = InnerInput.Id,
                MinLength = options.MinLength,
                Strict = options.Strict
            };
            var json = LaraUI.JSON.Stringify(payload);
            var code = $"LaraUI.autocompleteApply(context.Payload);";
            LaraUI.Page.JSBridge.Submit(code, json);
        }

        private string GetAutocompleteKey(Document document)
        {
            return InnerInput.Id + " " + document.VirtualIdString;
        }

        private void DestroyAutocomplete()
        {
            if (!_applied) return;
            _applied = false;
            AutocompleteService.Unregister(AutocompleteId);
            var code = $"LaraUI.autocompleteDestroy('{InnerInput.Id}');";
            LaraUI.Page.JSBridge.Submit(code);
        }

        /// <summary>
        /// Value property
        /// </summary>
        public string? Value
        {
            get => InnerInput.Value;
            set => InnerInput.Value = value;
        }
    }
}
