/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Specialized;
using System.ComponentModel;

namespace Integrative.Lara.Reactive
{
    class CollectionUpdater<T>
        where T : class, INotifyPropertyChanged
    {
        readonly BindChildrenOptions<T> _options;
        readonly Element _element;
        readonly NotifyCollectionChangedEventArgs _args;

        public CollectionUpdater(BindChildrenOptions<T> options,
            Element element,
            NotifyCollectionChangedEventArgs args)
        {
            _options = options;
            _element = element;
            _args = args;
        }

        public void Run()
        {
            switch (_args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    CollectionAdd();
                    break;
                case NotifyCollectionChangedAction.Move:
                    CollectionMove();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    CollectionRemove();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    CollectionReplace();
                    break;
                case NotifyCollectionChangedAction.Reset:
                default:
                    CollectionReset(_element);
                    break;
            }
        }

        private void CollectionAdd()
        {
            var item = (T)_args.NewItems[0];
            var callback = _options.CreateCallback;
            if (callback != null)
            {
                var childElement = callback(item);
                _element.AppendChild(childElement);
            }
        }

        private void CollectionMove()
        {
            var removeIndex = _args.OldStartingIndex;
            var addIndex = _args.NewStartingIndex;
            _element.SwapChildren(addIndex, removeIndex);
        }

        private void CollectionRemove()
        {
            var index = _args.OldStartingIndex;
            RemoveAt(index);
        }

        private void CollectionReplace()
        {
            var value = (T)_args.NewItems[0];
            var index = _args.OldStartingIndex;
            var callback = _options.CreateCallback;
            if (callback != null)
            {
                var childElement = callback(value);
                RemoveAt(index);
                InsertAt(index, childElement);
            }
        }

        private void RemoveAt(int index)
        {
            var child = _element.GetChildAt(index);
            if (child is Element element)
            {
                element.UnbindAll();
            }
            _element.RemoveAt(index);
        }

        private void InsertAt(int index, Element child)
        {
            _element.InsertChildAt(index, child);
        }

        private static void CollectionReset(Element element)
        {
            UnbindChildren(element);
            element.ClearChildren();
        }

        private static void UnbindChildren(Element element)
        {
            foreach (var node in element.Children)
            {
                if (node is Element childElement)
                {
                    childElement.UnbindAll();
                }
            }
        }

        public static void CollectionLoad(BindChildrenOptions<T> options, Element element)
        {
            CollectionReset(element);
            foreach (var item in options.Collection)
            {
                var callback = options.CreateCallback;
                if (callback != null)
                {
                    var child = callback(item);
                    element.AppendChild(child);
                }
            }
        }
    }
}
