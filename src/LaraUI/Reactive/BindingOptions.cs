/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Integrative.Lara
{
    /// <summary>
    /// Base class for binding options
    /// </summary>
    public abstract class BindOptions
    {
        internal abstract void Verify();
        internal abstract void Subscribe();
        internal abstract void Unsubscribe();
        internal abstract void Apply(Element element);

        internal static string MissingMemberText(string member)
        {
            return $"Missing binding options member: {member}";
        }
    }

    /// <summary>
    /// Base class for property-changed based bindings
    /// </summary>
    public abstract class BindPropertyOptions : BindOptions
    {
        internal event PropertyChangedEventHandler? PropertyChanged;

        internal virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        internal virtual void Collect(Element element)
        {
        }
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

        internal T GetBindObject()
        {
            if (BindObject == null)
            {
                throw new InvalidOperationException(MissingMemberText(nameof(BindObject)));
            }
            else
            {
                return BindObject;
            }
        }

        internal override void Subscribe()
        {
            GetBindObject().PropertyChanged += Object_PropertyChanged;
        }

        internal override void Unsubscribe()
        {
            GetBindObject().PropertyChanged -= Object_PropertyChanged;
        }

        private void Object_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e);
        }

        internal override void Verify()
        {
            if (BindObject == null)
            {
                throw new InvalidOperationException(MissingMemberText(nameof(BindObject)));
            }
        }
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

        private bool _applying;

        internal override void Apply(Element element)
        {
            _applying = true;
            ModifiedHandler?.Invoke(GetBindObject(), element);
            _applying = false;
        }

        internal override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            VerifyApplying();
            base.OnPropertyChanged(e);
        }

        private void VerifyApplying()
        {
            if (_applying)
            {
                throw new InvalidOperationException(Resources.BindingCycleDetected);
            }
        }

        internal override void Verify()
        {
            base.Verify();
            if (ModifiedHandler == null)
            {
                throw new InvalidOperationException(MissingMemberText(nameof(ModifiedHandler)));
            }
        }
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

        internal Func<TData, TValue> GetProperty
            => Property ?? throw new InvalidOperationException(MissingMemberText(nameof(Property)));

        internal TValue GetCurrentValue()
            => GetProperty(GetBindObject());

        internal override void Verify()
        {
            base.Verify();
            if (Property == null)
            {
                throw new InvalidOperationException(MissingMemberText(nameof(Property)));
            }
        }
    }

    /// <summary>
    /// Binding options for inner text
    /// </summary>
    /// <typeparam name="T">Type of data source object</typeparam>
    public sealed class BindInnerTextOptions<T> : BindPropertyOptions<T, string>
        where T : class, INotifyPropertyChanged
    {
        internal override void Apply(Element element)
        {
            var value = GetCurrentValue();
            element.InnerText = value;
        }
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
        where T : class, INotifyPropertyChanged
    {
        /// <summary>
        /// Attribute to bind
        /// </summary>
        public string Attribute { get; set; } = string.Empty;

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
        where T : class, INotifyPropertyChanged
    {
        /// <summary>
        /// Element class to toggle
        /// </summary>
        public string ClassName { get; set; } = string.Empty;

        internal override void Apply(Element element)
        {
            var value = GetCurrentValue();
            element.ToggleClass(ClassName, value);
        }
    }

    /// <summary>
    /// Base class for two-way binding
    /// </summary>
    /// <typeparam name="TData">Type of data source</typeparam>
    /// <typeparam name="TValue">Type of data property</typeparam>
    public abstract class BindInputOptions<TData, TValue> : BindPropertyOptions<TData>
        where TData : class, INotifyPropertyChanged
    {
        private string _attribute = string.Empty;

        /// <summary>
        /// Attribute to bind
        /// </summary>
        [SuppressMessage("Globalization",
            "CA1308:Normalize strings to uppercase", Justification = "html attributes are lowercase")]
        public string Attribute
        {
            get => _attribute;
            set
            {
                value = value ?? throw new ArgumentNullException(nameof(Attribute));
                _attribute = value.ToLowerInvariant();
            }
        }

        /// <summary>
        /// Bind model property
        /// </summary>
        public Expression<Func<TData, TValue>>? Property { get; set; }

        private Func<TData, TValue>? _getter;
        private Action<TData, TValue>? _setter;

        internal Expression<Func<TData, TValue>> GetProperty
            => Property ?? throw new InvalidOperationException(MissingMemberText(nameof(Property)));

        internal Func<TData, TValue> GetGetter()
        {
            if (_getter == null)
            {
                _getter = GetProperty.Compile();
            }
            return _getter;
        }
        
        internal Action<TData, TValue> GetSetter()
        {
            if (_setter == null)
            {
                _setter = CompileSetter();
            }
            return _setter;
        }

        internal TValue GetCurrentValue()
        {
            var getter = GetGetter();
            return getter(GetBindObject());
        }

        internal void SetValue(TValue value)
        {
            var setter = GetSetter();
            setter(GetBindObject(), value);
        }

        private Action<TData, TValue> CompileSetter()
        {
            var property = GetProperty;
            if (!(property.Body is MemberExpression member))
            {
                throw new ArgumentException(Resources.InvalidBindingExpression);
            }
            var param = Expression.Parameter(typeof(TValue), "value");
            var set = Expression.Lambda<Action<TData, TValue>>(
                Expression.Assign(member, param), property.Parameters[0], param);
            var action = set.Compile();
            return action;
        }

        internal override void Verify()
        {
            base.Verify();
            if (string.IsNullOrEmpty(_attribute))
            {
                throw new InvalidOperationException(MissingMemberText(nameof(_attribute)));
            }
            else if (Property == null)
            {
                throw new InvalidOperationException(MissingMemberText(nameof(Property)));
            }
            _getter = GetGetter();
            _setter = GetSetter();
        }
    }

    /// <summary>
    /// Binding options for two-way binding of attributes
    /// </summary>
    /// <typeparam name="T">Data source type</typeparam>
    public class BindInputOptions<T> : BindInputOptions<T, string?>
        where T : class, INotifyPropertyChanged
    {
        internal override void Apply(Element element)
        {
            var value = GetCurrentValue();
            element.SetAttributeLower(Attribute, value);
        }

        internal override void Collect(Element element)
        {
            var value = element.GetAttributeLower(Attribute) ?? string.Empty;
            SetValue(value);
        }
    }

    /// <summary>
    /// Binding options for two-way binding of flag attributes
    /// </summary>
    /// <typeparam name="T">Data source type</typeparam>
    public class BindFlagInputOptions<T> : BindInputOptions<T, bool>
        where T : class, INotifyPropertyChanged
    {
        internal override void Apply(Element element)
        {
            var value = GetCurrentValue();
            element.SetFlagAttributeLower(Attribute, value);
        }

        internal override void Collect(Element element)
        {
            var value = element.HasAttributeLower(Attribute);
            SetValue(value);
        }
    }

    /// <summary>
    /// Binding options for child element collections
    /// </summary>
    public abstract class BindChildrenOptions : BindOptions
    {
        internal abstract event NotifyCollectionChangedEventHandler? CollectionChanged;
        internal abstract void Apply(Element element, NotifyCollectionChangedEventArgs args);
    }

    /// <summary>
    /// Binding options for child element collections
    /// </summary>
    /// <typeparam name="T">Type of items in observable collection</typeparam>
    public sealed class BindChildrenOptions<T> : BindChildrenOptions
        where T : class, INotifyPropertyChanged
    {
        /// <summary>
        /// Collection that is tracked
        /// </summary>
        public ObservableCollection<T> Collection { get; }

        /// <summary>
        /// Method for creating elements
        /// </summary>
        public Func<T, Element>? CreateCallback { get; set; }

        internal override void Verify()
        {
            if (Collection == null)
            {
                throw new InvalidOperationException(MissingMemberText(nameof(Collection)));
            }
            else if (CreateCallback == null)
            {
                throw new InvalidOperationException(MissingMemberText(nameof(CreateCallback)));
            }
        }

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

        internal override event NotifyCollectionChangedEventHandler? CollectionChanged;

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
