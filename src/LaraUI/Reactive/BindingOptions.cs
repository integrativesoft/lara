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
    /// Base class for text binding options
    /// </summary>
    public abstract class BindTextOptions : BindOptions
    {
        internal abstract event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// Abstract class for text-property bindings
    /// </summary>
    /// <typeparam name="T">Type of data source</typeparam>
    public abstract class BindTextOptions<T> : BindTextOptions
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Instance to track changes
        /// </summary>
        public T Object { get; set; }

        /// <summary>
        /// Function to retrieve the target value from instance that's tracked
        /// </summary>
        public Func<T, string> Property { get; set; }

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

        internal string GetCurrentValue()
            => Property(Object);
    }

    /// <summary>
    /// Binding options for inner text
    /// </summary>
    /// <typeparam name="T">Type of data source object</typeparam>
    public sealed class BindInnerTextOptions<T> : BindTextOptions<T>
        where T : INotifyPropertyChanged
    {
        internal override void Apply(Element element)
        {
            var value = GetCurrentValue();
            element.ClearChildren();
            element.AppendText(value);
        }
    }

    /// <summary>
    /// Binding options for attributes
    /// </summary>
    /// <typeparam name="T">Type of data source object</typeparam>
    public sealed class BindAttributeOptions<T> : BindTextOptions<T>
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
