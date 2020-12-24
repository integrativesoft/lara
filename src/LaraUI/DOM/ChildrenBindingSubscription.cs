/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using System.Collections.Specialized;

namespace Integrative.Lara
{
    internal class ChildrenBindingSubscription
    {
        public NotifyCollectionChangedEventHandler Handler { get; }
        public INotifyCollectionChanged Source { get; }

        public ChildrenBindingSubscription(
            NotifyCollectionChangedEventHandler handler,
            INotifyCollectionChanged source)
        {
            Handler = handler;
            Source = source;
            source.CollectionChanged += handler;
        }

        public void Unsubscribe()
        {
            Source.CollectionChanged -= Handler;
        }
    }
}
