/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Integrative.Lara
{
    /// <summary>
    /// Extensions for element binding operations
    /// </summary>
    public static class BindingExtensions
    {
        #region bind properties

        /// <summary>
        /// Creates a binding to assign a property value when the source is modified
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <param name="source"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static TNode Bind<TNode>(
            this TNode node,
            INotifyPropertyChanged source,
            Action<TNode> handler)
            where TNode : Element
        {
            node = node ?? throw new ArgumentNullException(nameof(node));
            source = source ?? throw new ArgumentNullException(nameof(source));
            node.AddSubscription(source, () => handler(node));
            return node;
        }

        internal static Action<TNode, TValue> CompileSetter<TNode, TValue>(
            Expression<Func<TNode, TValue>> property)
        {
            if (property.Body is not MemberExpression member)
            {
                throw new ArgumentException(Resources.InvalidBindingExpression);
            }
            var param = Expression.Parameter(typeof(TValue), "value");
            var set = Expression.Lambda<Action<TNode, TValue>>(
                Expression.Assign(member, param), property.Parameters[0], param);
            return set.Compile();
        }

        #endregion

        #region bind input attributes

        /// <summary>
        /// Creates a two-way binding between an attribute and property
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="node"></param>
        /// <param name="changeNotifier"></param>
        /// <param name="propertyName"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static TNode BindInput<TNode>(
            this TNode node,
            INotifyPropertyChanged changeNotifier,
            string propertyName,
            string attribute)
            where TNode : Element
        {
            // verify parameters
            node = node ?? throw new ArgumentNullException(nameof(node));
            changeNotifier = changeNotifier
                ?? throw new ArgumentNullException(nameof(changeNotifier));
            var property = changeNotifier.GetType().GetProperty(propertyName)
                ?? throw new ArgumentException($"Property not found: {propertyName}");

            if (property.PropertyType == typeof(string))
            {
                // property change -> modify attribute
                node.AddSubscription(changeNotifier,
                    () => node.SetAttribute(attribute, property.GetValue(changeNotifier) as string));

                // modify property on attribute change
                node.SubscribeToAttribute(
                    attribute,
                    value => property.SetValue(changeNotifier, value));
            }
            else if (property.PropertyType == typeof(bool))
            {
                // property change -> modify attribute
                node.AddSubscription(changeNotifier, () =>
                {
                    var value = property.GetValue(changeNotifier) as bool?;
                    node.SetFlagAttribute(attribute, value ?? false);
                });

                // modify property on attribute change
                node.SubscribeToAttribute(
                    attribute,
                    _ => {
                        var flag = node.HasAttribute(attribute);
                        property.SetValue(changeNotifier, flag);
                    });
            }
            else
            {
                throw new ArgumentException(
                    "Only string and bool properties accepted when binding with attribute values"
                    );
            }

            return node;
        }

        #endregion

        #region bind children

        /// <summary>
        /// Bind element children with observable collection
        /// </summary>
        /// <typeparam name="TParent"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="element"></param>
        /// <param name="source"></param>
        /// <param name="childFactory"></param>
        /// <returns></returns>
        public static TParent BindChildren<TParent, TValue>(
            this TParent element,
            ObservableCollection<TValue> source,
            Func<TValue, Element> childFactory)
            where TParent : Element
            where TValue : class, INotifyPropertyChanged
        {
            element = element ?? throw new ArgumentNullException(nameof(element));
            source = source ?? throw new ArgumentNullException(nameof(source));
            element.ClearChildren();
            foreach (var item in source)
            {
                element.AppendChild(childFactory(item));
            }
            element.SubscribeChildren(source, (_, args) =>
            {
                var updater = new CollectionUpdater<TValue>(childFactory, element, args);
                updater.Run();
            });
            return element;
        }

        #endregion
    }
}
