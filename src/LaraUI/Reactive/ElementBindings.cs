/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Integrative.Lara.Reactive
{
    sealed class ElementBindings
    {
        private readonly Element _parent;
        private readonly Dictionary<string, BindTextOptions> _attributeBindings;

        private BindTextOptions _innerTextBinding;
        private BindChildrenOptions _childrenBinding;

        public ElementBindings(Element parent)
        {
            _parent = parent;
            _attributeBindings = new Dictionary<string, BindTextOptions>();
        }

        #region Common

        private void BindOptions(BindTextOptions options)
        {
            options.Apply(_parent);
            options.PropertyChanged += Options_PropertyChanged;
            options.Subscribe();
        }

        private void UnbindOptions(BindTextOptions options)
        {
            options.Unsubscribe();
            options.PropertyChanged -= Options_PropertyChanged;
        }

        private void Options_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var options = (BindTextOptions)sender;
            options.Apply(_parent);
        }

        public void UnbindAll()
        {
            UnbindAllAttributes();
            UnbindInnerText();
            UnbindChildren();
        }

        #endregion

        #region Attributes

        public void BindAttribute<T>(BindAttributeOptions<T> options)
            where T : INotifyPropertyChanged
        {
            UnbindAttribute(options.Attribute);
            BindOptions(options);
            _attributeBindings.Add(options.Attribute, options);
        }

        public void UnbindAttribute(string attribute)
        {
            if (_attributeBindings.TryGetValue(attribute, out var options))
            {
                UnbindOptions(options);
                _attributeBindings.Remove(attribute);
            }
        }

        private void UnbindAllAttributes()
        {
            var list = new List<string>(_attributeBindings.Keys);
            foreach (var attribute in list)
            {
                UnbindAttribute(attribute);
            }
        }

        #endregion

        #region Inner text

        public void BindInnerText<T>(BindTextOptions<T> options)
            where T : INotifyPropertyChanged
        {
            UnbindInnerText();
            BindOptions(options);
            _innerTextBinding = options;
        }

        public void UnbindInnerText()
        {
            if (_innerTextBinding != null)
            {
                UnbindOptions(_innerTextBinding);
                _innerTextBinding = null;
            }
        }

        #endregion

        #region Children

        public void BindChildren<T>(BindChildrenOptions<T> options)
            where T : INotifyPropertyChanged
        {
            UnbindChildren();
            options.Apply(_parent);
            options.CollectionChanged += Options_CollectionChanged;
            options.Subscribe();
        }

        public void UnbindChildren()
        {
            if (_childrenBinding != null)
            {
                _childrenBinding.Unsubscribe();
                _childrenBinding.CollectionChanged -= Options_CollectionChanged;
                _childrenBinding = null;
            }
        }

        private void Options_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var options = (BindChildrenOptions)sender;
            options.Apply(_parent);
        }

        #endregion
    }
}
