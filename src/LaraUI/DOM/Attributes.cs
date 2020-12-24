/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace Integrative.Lara
{
    internal sealed class Attributes : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Element _element;
        private readonly Dictionary<string, string?> _values;

        public Attributes(Element element)
        {
            _element = element;
            _values = new Dictionary<string, string?>();
        }

        public bool HasAttribute(string name) => HasAttributeLower(name.ToLowerInvariant());

        public string? GetAttribute(string name) => GetAttributeLower(name.ToLowerInvariant());

        internal bool HasAttributeLower(string nameLower)
            => _values.ContainsKey(nameLower);

        internal void SetAttributeLower(string nameLower, string? value)
        {
            if (nameLower == "slot" && _element.ParentElement != null)
            {
                throw new InvalidOperationException(Resources.SlotOnlyParent);
            }
            if (_values.TryGetValue(nameLower, out var previous))
            {
                if (previous == value)
                {
                    return;
                }
                _values.Remove(nameLower);
            }
            _values.Add(nameLower, value);
            if (nameLower == "value")
            {
                SetValueDelta.Enqueue(_element, value);
            }
            else if (nameLower == "checked")
            {
                SetCheckedDelta.Enqueue(_element, true);
            }
            else if (nameLower == "id")
            {
                SetIdDelta.Enqueue(_element, value);
            }
            else
            {
                AttributeEditedDelta.Enqueue(_element, nameLower, value);
            }
            _element.AttributeChanged(nameLower, value);
            if (nameLower == "slot")
            {
                _element.UpdateSlotted();
            }
        }

        internal void RemoveAttributeLower(string nameLower)
        {
            if (!_values.ContainsKey(nameLower))
            {
                return;
            }
            _values.Remove(nameLower);
            if (nameLower == "checked")
            {
                SetCheckedDelta.Enqueue(_element, false);
            }
            else
            {
                AttributeRemovedDelta.Enqueue(_element, nameLower);
            }
            _element.AttributeChanged(nameLower, null);
        }

        internal void SetFlagAttributeLower(string nameLower, bool value)
        {
            var current = _values.ContainsKey(nameLower);
            if (value == current) return;
            if (value)
            {
                SetAttributeLower(nameLower, "");
            }
            else
            {
                RemoveAttributeLower(nameLower);
            }
        }

        internal void NotifyValue(string value)
        {
            const string valueAttribute = "value";
            if (_values.TryGetValue(valueAttribute, out var previous))
            {
                if (previous == value)
                {
                    return;
                }
                _values.Remove(valueAttribute);
            }
            _values.Add(valueAttribute, value);
            _element.AttributeChanged(valueAttribute, value);
        }

        internal void NotifyChecked(bool isChecked)
        {
            NotifyFlag("checked", isChecked);
        }

        internal void NotifySelected(bool selected)
        {
            NotifyFlag("selected", selected);
        }

        private void NotifyFlag(string nameLower, bool value)
        {
            var current = _values.ContainsKey(nameLower);
            if (current == value)
            {
                return;
            }
            if (value)
            {
                _values.Add(nameLower, null);
            }
            else
            {
                _values.Remove(nameLower);
            }
            _element.AttributeChanged(nameLower, string.Empty);
        }

        internal string? GetAttributeLower(string nameLower)
        {
            return _values.TryGetValue(nameLower, out var result) ? result : string.Empty;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }
    }
}
