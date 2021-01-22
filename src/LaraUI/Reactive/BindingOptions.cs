/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Integrative.Lara
{
    /// <summary>
    /// Base class for binding options
    /// </summary>
    public abstract class BindOptions
    {
    }

    /// <summary>
    /// Base class for property-changed based bindings
    /// </summary>
    public abstract class BindPropertyOptions : BindOptions
    {
    }

    /// <summary>
    /// Base class for property-changed based bindings of source type T
    /// </summary>
    /// <typeparam name="T">Type of data source</typeparam>
    public abstract class BindPropertyOptions<T> : BindPropertyOptions
        where T : class, INotifyPropertyChanged
    {
        /// <summary>
        /// Instance to bind to
        /// </summary>
        public T? BindObject { get; set; }
    }

    /// <summary>
    /// Binding options for generic modification handler
    /// </summary>
    /// <typeparam name="T">Type of data source</typeparam>
    public sealed class BindHandlerOptions<T> : BindPropertyOptions<T>
        where T : class, INotifyPropertyChanged
    {
        /// <summary>
        /// Action to update the element whenever the data source is modified
        /// </summary>
        public Action<T, Element>? ModifiedHandler { get; set; }
    }

    /// <summary>
    /// Abstract class for text-property bindings
    /// </summary>
    /// <typeparam name="TData">Data type for data source instance</typeparam>
    /// <typeparam name="TValue">Data type for data source property</typeparam>
    public abstract class BindPropertyOptions<TData, TValue> : BindPropertyOptions<TData>
        where TData : class, INotifyPropertyChanged
    {
        /// <summary>
        /// Function to retrieve the target value from instance that's tracked
        /// </summary>
        public Func<TData, TValue>? Property { get; set; }
    }

    /// <summary>
    /// Binding options for inner text
    /// </summary>
    /// <typeparam name="T">Type of data source object</typeparam>
    public sealed class BindInnerTextOptions<T> : BindPropertyOptions<T, string>
        where T : class, INotifyPropertyChanged
    {
    }

    /// <summary>
    /// Binding options for attributes
    /// </summary>
    /// <typeparam name="T">Type of data source object</typeparam>
    public sealed class BindAttributeOptions<T> : BindPropertyOptions<T, string>
        where T : class, INotifyPropertyChanged
    {
        /// <summary>
        /// Attribute to bind
        /// </summary>
        public string Attribute { get; set; } = string.Empty;
    }

    /// <summary>
    /// Binding options for flag attributes
    /// </summary>
    /// <typeparam name="T">Source data type</typeparam>
    public sealed class BindFlagAttributeOptions<T> : BindPropertyOptions<T, bool>
        where T : class, INotifyPropertyChanged
    {
        /// <summary>
        /// Attribute to bind
        /// </summary>
        public string Attribute { get; set; } = string.Empty;
    }

    /// <summary>
    /// Binding options to toggle element classes
    /// </summary>
    /// <typeparam name="T">Source data type</typeparam>
    public sealed class BindToggleClassOptions<T> : BindPropertyOptions<T, bool>
        where T : class, INotifyPropertyChanged
    {
        /// <summary>
        /// Element class to toggle
        /// </summary>
        public string ClassName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Base class for two-way binding
    /// </summary>
    /// <typeparam name="TData">Type of data source</typeparam>
    /// <typeparam name="TValue">Type of data property</typeparam>
    public abstract class BindInputOptions<TData, TValue> : BindPropertyOptions<TData>
        where TData : class, INotifyPropertyChanged
    {
        /// <summary>
        /// Attribute to bind
        /// </summary>
        public string Attribute { get; set; } = "";

        /// <summary>
        /// Bind model property
        /// </summary>
        public Expression<Func<TData, TValue>>? Property { get; set; }
    }

    /// <summary>
    /// Binding options for two-way binding of attributes
    /// </summary>
    /// <typeparam name="T">Data source type</typeparam>
    public sealed class BindInputOptions<T> : BindInputOptions<T, string?>
        where T : class, INotifyPropertyChanged
    {
    }

    /// <summary>
    /// Binding options for two-way binding of flag attributes
    /// </summary>
    /// <typeparam name="T">Data source type</typeparam>
    public sealed class BindFlagInputOptions<T> : BindInputOptions<T, bool>
        where T : class, INotifyPropertyChanged
    {
    }

    /// <summary>
    /// Binding options for child element collections
    /// </summary>
    public abstract class BindChildrenOptions : BindOptions
    {
    }

    /// <summary>
    /// Binding options for child element collections
    /// </summary>
    /// <typeparam name="T">Type of items in observable collection</typeparam>
    public sealed class BindChildrenOptions<T> : BindChildrenOptions
    {
        /// <summary>
        /// Collection that is tracked
        /// </summary>
        public ObservableCollection<T> Collection { get; }

        /// <summary>
        /// Method for creating elements
        /// </summary>
        public Func<T, Element> CreateCallback { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collection">Collection to bind</param>
        /// <param name="createCallback">Method for creating elements</param>
        public BindChildrenOptions(ObservableCollection<T> collection, Func<T, Element> createCallback)
        {
            CreateCallback = createCallback;
            Collection = collection;
        }
    }
}
