/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Integrative.Lara
{
    /// <summary>
    /// Input element
    /// </summary>
    [Obsolete("Use HtmlInputElement instead")]
    public class InputElement : HtmlInputElement
    {
    }

    /// <summary>
    /// The 'input' HTML5 element
    /// </summary>
    /// <seealso cref="Element" />
    public class HtmlInputElement : Element
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlInputElement"/> class.
        /// </summary>
        public HtmlInputElement() : base("input")
        {
        }

        internal override void NotifyValue(ElementEventValue entry)
        {
            BeginUpdate();
            base.NotifyValue(entry);
            NotifyValue(entry.Value);
            NotifyChecked(entry.Checked);
            EndUpdate();
            ClearFiles();
        }

        /// <summary>
        /// Gets or sets the 'accept' HTML5 attribute.
        /// </summary>
        public string? Accept
        {
            get => GetAttributeLower("accept");
            set => SetAttributeLower("accept", value);
        }

        /// <summary>
        /// Gets or sets the 'alt' HTML5 attribute.
        /// </summary>
        public string? Alt
        {
            get => GetAttributeLower("alt");
            set => SetAttributeLower("alt", value);
        }

        /// <summary>
        /// Gets or sets the 'autocomplete' HTML5 attribute.
        /// </summary>
        public string? Autocomplete
        {
            get => GetAttributeLower("autocomplete");
            set => SetAttributeLower("autocomplete", value);
        }

        /// <summary>
        /// Gets or sets the 'autofocus' HTML5 attribute.
        /// </summary>
        public bool Autofocus
        {
            get => HasAttributeLower("autofocus");
            set => SetFlagAttributeLower("autofocus", value);
        }

        /// <summary>
        /// Gets or sets the 'checked' HTML5 attribute.
        /// </summary>
        public bool Checked
        {
            get => HasAttributeLower("checked");
            set => SetFlagAttributeLower("checked", value);
        }

        /// <summary>
        /// Gets or sets the 'dirname' HTML5 attribute.
        /// </summary>
        public string? Dirname
        {
            get => GetAttributeLower("dirname");
            set => SetAttributeLower("dirname", value);
        }

        /// <summary>
        /// Gets or sets the 'disabled' HTML5 attribute.
        /// </summary>
        public bool Disabled
        {
            get => HasAttributeLower("disabled");
            set => SetFlagAttributeLower("disabled", value);
        }

        /// <summary>
        /// Gets or sets the 'height' HTML5 attribute.
        /// </summary>
        public int? Height
        {
            get => GetIntAttribute("height");
            set => SetIntAttribute("height", value);
        }

        /// <summary>
        /// Gets or sets the 'list' HTML5 attribute.
        /// </summary>
        public string? List
        {
            get => GetAttributeLower("list");
            set => SetAttributeLower("list", value);
        }

        /// <summary>
        /// Gets or sets the 'max' HTML5 attribute.
        /// </summary>
        public string? Max
        {
            get => GetAttributeLower("max");
            set => SetAttributeLower("max", value);
        }

        /// <summary>
        /// Gets or sets the 'maxlength' HTML5 attribute.
        /// </summary>
        public int? MaxLength
        {
            get => GetIntAttribute("maxlength");
            set => SetIntAttribute("maxlength", value);
        }

        /// <summary>
        /// Gets or sets the 'min' HTML5 attribute.
        /// </summary>
        public string? Min
        {
            get => GetAttributeLower("min");
            set => SetAttributeLower("min", value);
        }

        /// <summary>
        /// Gets or sets the 'multiple' HTML5 attribute.
        /// </summary>
        public bool Multiple
        {
            get => HasAttributeLower("multiple");
            set => SetFlagAttributeLower("multiple", value);
        }

        /// <summary>
        /// Gets or sets the 'name' HTML5 attribute.
        /// </summary>
        public string? Name
        {
            get => GetAttributeLower("name");
            set => SetAttributeLower("name", value);
        }

        /// <summary>
        /// Gets or sets the 'pattern' HTML5 attribute.
        /// </summary>
        public string? Pattern
        {
            get => GetAttributeLower("pattern");
            set => SetAttributeLower("pattern", value);
        }

        /// <summary>
        /// Gets or sets the 'placeholder' HTML5 attribute.
        /// </summary>
        public string? Placeholder
        {
            get => GetAttributeLower("placeholder");
            set => SetAttributeLower("placeholder", value);
        }

        /// <summary>
        /// Gets or sets the 'readonly' HTML5 attribute.
        /// </summary>
        public bool Readonly
        {
            get => HasAttributeLower("readonly");
            set => SetFlagAttributeLower("readonly", value);
        }

        /// <summary>
        /// Gets or sets the 'required' HTML5 attribute.
        /// </summary>
        public bool Required
        {
            get => HasAttributeLower("required");
            set => SetFlagAttributeLower("required", value);
        }

        /// <summary>
        /// Gets or sets the 'size' HTML5 attribute.
        /// </summary>
        public int? Size
        {
            get => GetIntAttribute("size");
            set => SetIntAttribute("size", value);
        }

        /// <summary>
        /// Gets or sets the 'src' HTML5 attribute.
        /// </summary>
        public string? Src
        {
            get => GetAttributeLower("src");
            set => SetAttributeLower("src", value);
        }

        /// <summary>
        /// Gets or sets the 'step' HTML5 attribute.
        /// </summary>
        public string? Step
        {
            get => GetAttributeLower("step");
            set => SetAttributeLower("step", value);
        }

        /// <summary>
        /// Gets or sets the 'type' HTML5 attribute.
        /// </summary>
        public string? Type
        {
            get => GetAttributeLower("type");
            set => SetAttributeLower("type", value);
        }

        /// <summary>
        /// Gets or sets the 'value' HTML5 attribute.
        /// </summary>
        public string? Value
        {
            get => GetAttributeLower("value");
            set => SetAttributeLower("value", value);
        }

        /// <summary>
        /// Gets or sets the 'width' HTML5 attribute.
        /// </summary>
        public int? Width
        {
            get => GetIntAttribute("width");
            set => SetIntAttribute("width", value);
        }

        private readonly List<IFormFile> _files = new List<IFormFile>();
        private void ClearFiles() => _files.Clear();
        internal void AddFile(IFormFile file) => _files.Add(file);

        /// <summary>
        /// Collection of uploaded files for input elements with type='file'
        /// </summary>
        public IReadOnlyList<IFormFile> Files => _files;
    }
}
