/*
Copyright (c) 2021 Integrative Software LLC
Created: 1/2021
Author: Pablo Carbonell
*/

using System;
using System.Collections.ObjectModel;

namespace Integrative.Lara
{
    /// <summary>
    /// Fragment component to group element in a single parent
    /// </summary>
    public class Fragment : WebComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Fragment()
        {
            ShadowRoot.AppendChild(new Slot());
        }

        /// <summary>
        /// Creates a Fragment with a list of children that follows an observable collection
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static Fragment ForEach<TValue>(ObservableCollection<TValue> source, Func<TValue, Node> factory)
        {
            var fragment = new Fragment();
            fragment.BindChildren(source, factory);
            return fragment;
        }
    }
}
