/*
Copyright (c) 2019 Integrative Software LLC
Created: 9/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Tools;
using System;
using System.Collections.Generic;

namespace Integrative.Lara
{
    /// <summary>
    /// Represents ambient data that is local to the current document.
    /// </summary>
    /// <typeparam name="T">Value</typeparam>
    public class DocumentLocal<T>
    {
        readonly Dictionary<Document, T> _storage;

        /// <summary>
        /// Constructor
        /// </summary>
        public DocumentLocal()
        {
            _storage = new Dictionary<Document, T>();
        }

        /// <summary>
        /// Value
        /// </summary>
        public T Value
        {
            get => GetValue();
            set => SetValue(value);
        }

        private T GetValue()
        {
            var document = GetDocument();
            _storage.TryGetValue(document, out var value);
            return value;
        }

        private void SetValue(T value)
        {
            var document = GetDocument();
            if (_storage.TryGetValue(document, out var previous))
            {
                if (LaraTools.SameValue(previous, value))
                {
                    return;
                }
                _storage.Remove(document);
                _storage.Add(document, value);
            }
            else
            {
                _storage.Add(document, value);
                document.AfterUnload += (sender, args) => _storage.Remove(document);
            }
        }

        private static Document GetDocument()
        {
            if (LaraUI.Page == null)
            {
                throw new InvalidOperationException(Resources.NoCurrentDocument);
            }
            return LaraUI.Page.Document;
        }
    }
}
