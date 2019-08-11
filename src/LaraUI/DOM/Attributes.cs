/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Delta;
using System.Collections;
using System.Collections.Generic;

namespace Integrative.Lara.DOM
{
    sealed class Attributes : IEnumerable<KeyValuePair<string, string>>
    {
        readonly Element _element;
        readonly Dictionary<string, string> _values;

        public Attributes(Element element)
        {
            _element = element;
            _values = new Dictionary<string, string>();
        }

        public bool HasAttribute(string name) => HasAttributeLower(name.ToLower());

        public string GetAttribute(string name) => GetAttributeLower(name.ToLower());

        internal bool HasAttributeLower(string nameLower) => _values.ContainsKey(nameLower);

        internal void SetAttributeLower(string nameLower, string value)
        {
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
            _element.AttributeChanged(nameLower);
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
            _element.AttributeChanged(nameLower);
        }

        internal void SetFlagAttributeLower(string nameLower, bool value)
        {
            bool current = _values.ContainsKey(nameLower);
            if (value != current)
            {
                if (value)
                {
                    SetAttributeLower(nameLower, null);
                }
                else
                {
                    RemoveAttributeLower(nameLower);
                }
            }
        }

        internal void NotifyValue(string value)
        {
            const string ValueAttribute = "value";
            if (_values.TryGetValue(ValueAttribute, out var previous))
            {
                if (previous == value)
                {
                    return;
                }
                _values.Remove(ValueAttribute);
            }
            _values.Add(ValueAttribute, value);
            _element.AttributeChanged(ValueAttribute);
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
            bool current = _values.ContainsKey(nameLower);
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
            _element.AttributeChanged(nameLower);
        }

        internal string GetAttributeLower(string nameLower)
        {
            if (_values.TryGetValue(nameLower, out var result))
            {
                return result;
            }
            else
            {
                return string.Empty;
            }
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
