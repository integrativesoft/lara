/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Reactive;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Integrative.Lara
{
    /// <summary>
    /// Base class for binding options
    /// </summary>
    public abstract class BindOptions
    {
        internal abstract void Subscribe();
        internal abstract void Unsubscribe();
        internal abstract void Apply(Element element);
    }

    /// <summary>
    /// Base class for property-changed based bindings
    /// </summary>
    public abstract class BindPropertyOptions : BindOptions
    {
        internal abstract event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// Binding options for generic modification handler
    /// </summary>
    /// <typeparam name="T">Type of data source</typeparam>
    public sealed class BindHandlerOptions<T> : BindPropertyOptions
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Instance to bind to
        /// </summary>
        public T Object { get; set; }

        /// <summary>
        /// Action to update the element whenever the data source is modified
        /// </summary>
        public Action<T, Element> ModifiedHandler { get; set; }

        internal override event PropertyChangedEventHandler PropertyChanged;

        private bool _applying;

        internal override void Apply(Element element)
        {
            _applying = true;
            ModifiedHandler?.Invoke(Object, element);
            _applying = false;
        }

        internal override void Subscribe()
        {
            Object.PropertyChanged += Object_PropertyChanged;
        }

        internal override void Unsubscribe()
        {
            Object.PropertyChanged -= Object_PropertyChanged;
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            VerifyApplying();
            PropertyChanged?.Invoke(this, e);
        }

        private void VerifyApplying()
        {
            const string Template = "Cycle detected: modification handlers should not modify the source data.";
            if (_applying)
            {
                throw new InvalidOperationException(Template);
            }
        }
    }

    /// <summary>
    /// Abstract class for text-property bindings
    /// </summary>
    /// <typeparam name="TData">Data type for data source instance</typeparam>
    /// <typeparam name="TValue">Data type for data source property</typeparam>
    public abstract class BindPropertyOptions<TData, TValue> : BindPropertyOptions
        where TData : INotifyPropertyChanged
    {
        /// <summary>
        /// Instance to track changes
        /// </summary>
        public TData Object { get; set; }

        /// <summary>
        /// Function to retrieve the target value from instance that's tracked
        /// </summary>
        public Func<TData, TValue> Property { get; set; }

        internal override event PropertyChangedEventHandler PropertyChanged;

        internal override void Subscribe()
        {
            Object.PropertyChanged += ObjectChangedHandler;
        }

        internal override void Unsubscribe()
        {
            Object.PropertyChanged -= ObjectChangedHandler;
        }

        private void ObjectChangedHandler(object sender, PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        internal TValue GetCurrentValue()
            => Property(Object);
    }

    /// <summary>
    /// Binding options for inner text
    /// </summary>
    /// <typeparam name="T">Type of data source object</typeparam>
    public sealed class BindInnerTextOptions<T> : BindPropertyOptions<T, string>
        where T : INotifyPropertyChanged
    {
        internal override void Apply(Element element)
        {
            var value = GetCurrentValue();
            element.SetInnerText(value);
        }
    }

    /// <summary>
    /// Binding options for attributes
    /// </summary>
    /// <typeparam name="T">Type of data source object</typeparam>
    public sealed class BindAttributeOptions<T> : BindPropertyOptions<T, string>
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Attribute to bind
        /// </summary>
        public string Attribute { get; set; }

        internal override void Apply(Element element)
        {
            var value = GetCurrentValue();
            element.SetAttribute(Attribute, value);
        }
    }

    /// <summary>
    /// Binding options for flag attributes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class BindFlagAttributeOptions<T> : BindPropertyOptions<T, bool>
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Attribute to bind
        /// </summary>
        public string Attribute { get; set; }

        internal override void Apply(Element element)
        {
            var value = GetCurrentValue();
            element.SetFlagAttribute(Attribute, value);
        }
    }

    /// <summary>
    /// Binding options for child element collections
    /// </summary>
    public abstract class BindChildrenOptions : BindOptions
    {
        internal abstract event NotifyCollectionChangedEventHandler CollectionChanged;
        internal abstract void Apply(Element element, NotifyCollectionChangedEventArgs args);
    }

    /// <summary>
    /// Binding options for child element collections
    /// </summary>
    /// <typeparam name="T">Type of items in observable collection</typeparam>
    public sealed class BindChildrenOptions<T> : BindChildrenOptions
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Collection that is tracked
        /// </summary>
        public ObservableCollection<T> Collection { get; set; }

        /// <summary>
        /// Method for creating elements
        /// </summary>
        public Func<T, Element> CreateCallback { get; set; }

        internal override event NotifyCollectionChangedEventHandler CollectionChanged;

        internal override void Apply(Element element, NotifyCollectionChangedEventArgs args)
        {
            var updater = new CollectionUpdater<T>(this, element, args);
            updater.Run();
        }

        internal override void Subscribe()
        {
            Collection.CollectionChanged += CollectionChangedHandler;
        }

        internal override void Unsubscribe()
        {
            Collection.CollectionChanged -= CollectionChangedHandler;
        }

        private void CollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }

        internal override void Apply(Element element)
        {
            CollectionUpdater<T>.CollectionLoad(this, element);
        }
    }
}
