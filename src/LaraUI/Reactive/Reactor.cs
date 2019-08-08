/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/
/*
using Integrative.Lara.Reactive;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Integrative.Lara
{
    /// <summary>
    /// The 'Reactor' class that implements bindings and reactive programming
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Reactor<T>
        where T : INotifyPropertyChanged
    {
        readonly TwoWayDictionary<Element, T> _objects;

        /// <summary>
        /// Callback for updating an element whenever its associated object was modified
        /// </summary>
        public Action<Element, T> ModifiedCallback { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Reactor()
        {
            _objects = new TwoWayDictionary<Element, T>();
        }

        /// <summary>
        /// Registers an association between an element and an object
        /// </summary>
        /// <param name="element">Element to update</param>
        /// <param name="value">Source object with data</param>
        public void Bind(Element element, T value)
        {
            _objects.Add(element, value);
            value.PropertyChanged += Value_Modified;
        }

        /// <summary>
        /// Removes bindings for an element
        /// </summary>
        /// <param name="element">Element</param>
        public void Remove(Element element)
        {
            if (_objects.TryGetValue(element, out var value))
            {
                _objects.Remove(element);
                value.PropertyChanged -= Value_Modified;
                OnRemove(element);
            }
        }

        /// <summary>
        /// Used for derived reactors only
        /// </summary>
        /// <param name="element"></param>
        protected virtual void OnRemove(Element element)
        {
        }

        private void Value_Modified(object sender, EventArgs e)
        {
            if (ModifiedCallback == null)
            {
                return;
            }
            var value = (T)sender;
            foreach (var element in _objects.GetChildKeys(value))
            {
                ModifiedCallback(element, value);
            }
        }
    }

    /// <summary>
    /// Reactor for objects of type T with children of type TChild
    /// </summary>
    /// <typeparam name="T">Type to associate with elements</typeparam>
    /// <typeparam name="TChild">Type to associate with children of each element</typeparam>
    public class Reactor<T, TChild> : Reactor<T>
        where T : INotifyPropertyChanged
        where TChild : INotifyPropertyChanged
    {
        readonly TwoWayDictionary<Element, ObservableCollection<TChild>> _collections;

        /// <summary>
        /// Function to create elements when needed
        /// </summary>
        public Func<TChild, Element> ChildCreateCallback { get; set; }

        /// <summary>
        /// Function to notify when elements are removed
        /// </summary>
        public Action<Element> ChildDeleteCallback { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Reactor() : base()
        {
            _collections = new TwoWayDictionary<Element, ObservableCollection<TChild>>();
        }

        /// <summary>
        /// Bind an element's children list with an observable collection
        /// </summary>
        /// <param name="element">Element to bind</param>
        /// <param name="collection">Observable collection with source data</param>
        public void BindChildren(Element element, ObservableCollection<TChild> collection)
        {
            UnbindChildren(element);
            _collections.Add(element, collection);
            collection.CollectionChanged += Collection_CollectionChanged;
        }

        /// <summary>
        /// Method used by derived reactors
        /// </summary>
        /// <param name="element"></param>
        protected override void OnRemove(Element element)
        {
            base.OnRemove(element);
            UnbindChildren(element);
        }

        /// <summary>
        /// Removes bindings for children for a given element
        /// </summary>
        /// <param name="element">Element for which we remove bindings</param>
        public void UnbindChildren(Element element)
        {
            if (_collections.TryGetValue(element, out var collection))
            {
                _collections.Remove(element);
                collection.CollectionChanged -= Collection_CollectionChanged;
            }
        }

        private void Collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collection = (ObservableCollection<TChild>)sender;
            foreach (var element in _collections.GetChildKeys(collection))
            {
                var updater = new CollectionUpdater<T, TChild>(this, element, e);
                updater.Run();
            }
        }
    }
}
*/