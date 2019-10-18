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
using System.Linq.Expressions;

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
        public T BindObject { get; set; }

        /// <summary>
        /// Action to update the element whenever the data source is modified
        /// </summary>
        public Action<T, Element> ModifiedHandler { get; set; }

        internal override event PropertyChangedEventHandler PropertyChanged;

        private bool _applying;

        internal override void Apply(Element element)
        {
            _applying = true;
            ModifiedHandler?.Invoke(BindObject, element);
            _applying = false;
        }

        internal override void Subscribe()
        {
            BindObject.PropertyChanged += Object_PropertyChanged;
        }

        internal override void Unsubscribe()
        {
            BindObject.PropertyChanged -= Object_PropertyChanged;
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            VerifyApplying();
            PropertyChanged?.Invoke(this, e);
        }

        private void VerifyApplying()
        {
            if (_applying)
            {
                throw new InvalidOperationException(Resources.BindingCycleDetected);
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
        public TData BindObject { get; set; }

        /// <summary>
        /// Function to retrieve the target value from instance that's tracked
        /// </summary>
        public Func<TData, TValue> Property { get; set; }

        internal override event PropertyChangedEventHandler PropertyChanged;

        internal override void Subscribe()
        {
            BindObject.PropertyChanged += ObjectChangedHandler;
        }

        internal override void Unsubscribe()
        {
            BindObject.PropertyChanged -= ObjectChangedHandler;
        }

        private void ObjectChangedHandler(object sender, PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }

        internal TValue GetCurrentValue()
            => Property(BindObject);
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
    /// <typeparam name="T">Source data type</typeparam>
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
    /// Binding options to toggle element classes
    /// </summary>
    /// <typeparam name="T">Source data type</typeparam>
    public sealed class BindToggleClassOptions<T> : BindPropertyOptions<T, bool>
        where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Element class to toggle
        /// </summary>
        public string ClassName { get; set; }

        internal override void Apply(Element element)
        {
            var value = GetCurrentValue();
            element.ToggleClass(ClassName, value);
        }
    }

    /// <summary>
    /// Base class for two-way binding
    /// </summary>
    public abstract class BindTwoWayOptions<TData, TValue> : BindPropertyOptions
        where TData : INotifyPropertyChanged
    {
        /// <summary>
        /// Bind model property
        /// </summary>
        public Expression<Func<TData, TValue>> Property { get; set; }

        /// <summary>
        /// Instance to track changes
        /// </summary>
        public TData BindObject { get; set; }

        internal TValue GetCurrentValue()
        {
            var getter = Property.Compile();
            return getter(BindObject);
        }

        internal void SetValue(TValue value)
        {
            var member = (MemberExpression)Property.Body;
            var param = Expression.Parameter(typeof(TValue), "value");
            var set = Expression.Lambda<Action<TData, TValue>>(
                Expression.Assign(member, param), Property.Parameters[0], param);
            var action = set.Compile();
            action(BindObject, value);
        }

        internal abstract void Collect(Element element);

        internal override event PropertyChangedEventHandler PropertyChanged;

        internal override void Subscribe()
        {
            BindObject.PropertyChanged += ObjectChangedHandler;
        }

        internal override void Unsubscribe()
        {
            BindObject.PropertyChanged -= ObjectChangedHandler;
        }

        private void ObjectChangedHandler(object sender, PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
    }

    /// <summary>
    /// Binding options for two-way binding of element 'value' property
    /// </summary>
    /// <typeparam name="T">Source data type</typeparam>
    public sealed class BindValueOptions<T> : BindTwoWayOptions<T, string>
        where T : INotifyPropertyChanged
    {
        internal override void Apply(Element element)
        {
            var value = GetCurrentValue();
            element.SetAttributeLower("value", value);
        }

        internal override void Collect(Element element)
        {
            var value = element.GetAttributeLower("value");
            SetValue(value);
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
        public ObservableCollection<T> Collection { get; }

        /// <summary>
        /// Method for creating elements
        /// </summary>
        public Func<T, Element> CreateCallback { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collection">Collection to bind</param>
        public BindChildrenOptions(ObservableCollection<T> collection)
        {
            Collection = collection;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collection">Collection to bind</param>
        /// <param name="createCallback">Method for creating elements</param>
        public BindChildrenOptions(ObservableCollection<T> collection, Func<T, Element> createCallback)
            : this(collection)
        {
            CreateCallback = createCallback;
        }

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
