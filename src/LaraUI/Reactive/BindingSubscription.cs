/*
Copyright (c) 2020-2021 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using System.ComponentModel;

namespace Integrative.Lara
{
    internal class BindingSubscription
    {
        public INotifyPropertyChanged Source { get; }
        public PropertyChangedEventHandler Handler { get; }

        public BindingSubscription(
            INotifyPropertyChanged source,
            PropertyChangedEventHandler handler)
        {
            Source = source;
            Handler = handler;
            Source.PropertyChanged += handler;
        }

        public void Unsubscribe() => Source.PropertyChanged -= Handler;
    }
}
