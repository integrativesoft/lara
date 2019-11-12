/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Autocomplete;

namespace Integrative.Lara
{
    /// <summary>
    /// Autocomplete web component
    /// </summary>
    [LaraWebComponent(CustomTag)]
    public class AutocompleteElement : WebComponent
    {
        /// <summary>
        /// Autocomplete's custom HTML tag
        /// </summary>
        public const string CustomTag = "lara-autocomplete";

        /// <summary>
        /// Returns the inner input element
        /// </summary>
        public InputElement InnerInput { get; } = new InputElement();

        /// <summary>
        /// Constructor
        /// </summary>
        public AutocompleteElement() : base(CustomTag)
        {
            InnerInput.Autocomplete = "off";
            ShadowRoot.AppendChild(InnerInput);
        }

        bool _pending, _applied;

        AutocompleteOptions _options;

        internal AutocompleteOptions GetOptions() => _options;

        /// <summary>
        /// Enables the autocomplete functionality
        /// </summary>
        public void Start(AutocompleteOptions options)
        {
            _options = options;
            if (Document == null)
            {
                _pending = true;
            }
            else
            {
                DestroyAutocomplete();
                SubmitAutocomplete();
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
            if (_pending)
            {
                _pending = false;
                DestroyAutocomplete();
                SubmitAutocomplete();
            }
        }

        /// <summary>
        /// Disposes autocomplete references on browser
        /// </summary>
        protected override void OnDisconnect()
        {
            DestroyAutocomplete();
        }

        internal string AutocompleteId { get; set; }

        private void SubmitAutocomplete()
        { 
            AutocompleteId = GetAutocompleteKey();
            AutocompleteService.Register(AutocompleteId, this);
            _applied = true;
            _pending = false;
            var payload = new AutocompletePayload
            {
                AutoFocus = _options.AutoFocus,
                ElementId = InnerInput.EnsureElementId(),
                MinLength = _options.MinLength,
                Strict = _options.Strict
            };
            var json = LaraUI.JSON.Stringify(payload);
            var code = $"LaraUI.autocompleteApply(context.Payload);";
            LaraUI.Page.JSBridge.Submit(code, json);
        }

        private string GetAutocompleteKey()
        {
            return InnerInput.EnsureElementId() + " " + Document.VirtualIdString;
        }

        private void DestroyAutocomplete()
        {
            if (_applied)
            {
                _applied = false;
                AutocompleteService.Unregister(AutocompleteId);
                var code = $"LaraUI.autocompleteDestroy('{InnerInput.Id}');";
                LaraUI.Page.JSBridge.Submit(code);
            }
        }

        /// <summary>
        /// Value property
        /// </summary>
        public string Value
        {
            get => InnerInput.Value;
            set => InnerInput.Value = value;
        }
    }
}
