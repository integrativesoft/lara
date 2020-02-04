/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Integrative.Lara
{
    internal sealed class ElementBindings
    {
        private readonly Element _parent;

        private readonly Dictionary<string, BindPropertyOptions> _attributeBindings;
        private BindPropertyOptions? _innerTextBinding;
        private BindChildrenOptions? _childrenBinding;
        private BindPropertyOptions? _genericHandler;

        public ElementBindings(Element parent)
        {
            _parent = parent;
            _attributeBindings = new Dictionary<string, BindPropertyOptions>();
        }

        #region Common

        private void BindOptions(BindPropertyOptions options)
        {
            options.Apply(_parent);
            options.PropertyChanged += Options_PropertyChanged;
            options.Subscribe();
        }

        private void UnbindOptions(BindPropertyOptions options)
        {
            options.Unsubscribe();
            options.PropertyChanged -= Options_PropertyChanged;
        }

        private void Options_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var options = (BindPropertyOptions)sender;
            options.Apply(_parent);
        }

        public void UnbindAll()
        {
            UnbindHandler();
            UnbindAllAttributes();
            UnbindInnerText();
            UnbindChildren();
        }

        public void NotifyAttributeChanged(string attribute)
        {
            if(_attributeBindings.TryGetValue(attribute, out var binding))
            {
                binding.Collect(_parent);
            }
        }

        #endregion

        #region Generic handler

        public void BindHandler<T>(BindHandlerOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            UnbindHandler();
            BindOptions(options);
            _genericHandler = options;
        }

        public void UnbindHandler()
        {
            if (_genericHandler == null) return;
            UnbindOptions(_genericHandler);
            _genericHandler = null;
        }

        #endregion

        #region Attributes

        public void BindAttribute<T>(BindAttributeOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            StoreBinding(options.Attribute, options);
        }

        public void BindFlagAttribute<T>(BindFlagAttributeOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            StoreBinding(options.Attribute, options);
        }

        public void BindToggleClass<T>(BindToggleClassOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            var key = "class~" + options.ClassName;
            StoreBinding(key, options);
        }

        public void BindInput<TData, TValue>(BindInputOptions<TData, TValue> options)
            where TData : class, INotifyPropertyChanged
        {
            StoreBinding(options.Attribute, options);
        }

        private void StoreBinding(string key, BindPropertyOptions options)
        {
            UnbindAttribute(key);
            BindOptions(options);
            _attributeBindings.Add(key, options);
        }

        public void UnbindAttribute(string attribute)
        {
            if (!_attributeBindings.TryGetValue(attribute, out var options)) return;
            UnbindOptions(options);
            _attributeBindings.Remove(attribute);
        }

        public void UnbindAllAttributes()
        {
            var list = new List<string>(_attributeBindings.Keys);
            foreach (var attribute in list)
            {
                UnbindAttribute(attribute);
            }
        }

        #endregion

        #region Inner text

        public void BindInnerText<T>(BindPropertyOptions<T, string> options)
            where T : class, INotifyPropertyChanged
        {
            UnbindInnerText();
            BindOptions(options);
            _innerTextBinding = options;
        }

        public void UnbindInnerText()
        {
            if (_innerTextBinding == null) return;
            UnbindOptions(_innerTextBinding);
            _innerTextBinding = null;
        }

        #endregion

        #region Children

        public void BindChildren<T>(BindChildrenOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            UnbindChildren();
            options.Apply(_parent);
            options.CollectionChanged += Options_CollectionChanged;
            options.Subscribe();
            _childrenBinding = options;
        }

        public void UnbindChildren()
        {
            if (_childrenBinding == null) return;
            _childrenBinding.Unsubscribe();
            _childrenBinding.CollectionChanged -= Options_CollectionChanged;
            _childrenBinding = null;
        }

        private void Options_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var options = (BindChildrenOptions)sender;
            options.Apply(_parent, e);
        }

        #endregion
    }
}
