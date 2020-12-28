/*
Copyright (c) 2020 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Integrative.Lara
{
    /// <summary>
    /// Obsolete methods on Element class
    /// </summary>
    public static class ObsoleteElement
    {
        /// <summary>
        /// Binds an element to an action to be triggered whenever the source data changes
        /// </summary>
        /// <typeparam name="T">Type of the source data</typeparam>
        /// <param name="self"></param>
        /// <param name="options">Binding options</param>
        [Obsolete("Use OnSourceChange instead")]
        public static void Bind<T>(this Element self, BindHandlerOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            options = options ?? throw new ArgumentNullException(nameof(options));
            var handler = options.ModifiedHandler ?? throw new ArgumentNullException(nameof(options.ModifiedHandler));
            var source = options.BindObject ?? throw new ArgumentNullException(nameof(options.BindObject));

            self.OnSourceChange(source, _ => handler(source, self));
        }

        /// <summary>
        /// Binds an attribute
        /// </summary>
        /// <typeparam name="T">Data type for data source instance</typeparam>
        /// <param name="self"></param>
        /// <param name="options">Attribute binding options</param>
        [Obsolete("Use OnSourceChange instead")]
        public static void BindAttribute<T>(this Element self, BindAttributeOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            var source = options.BindObject ?? throw new ArgumentNullException(nameof(options.BindObject));
            var property = options.Property ?? throw new ArgumentNullException(nameof(options.Property));
            var attribute = options.Attribute;
            self.OnSourceChange(source, x => x.SetAttribute(attribute, property(source)));
        }

        /// <summary>
        /// Binds a flag attribute
        /// </summary>
        /// <typeparam name="T">Data type for data source instance</typeparam>
        /// <param name="self"></param>
        /// <param name="options">Binding options</param>
        [Obsolete("Use OnSourceChange instead")]
        public static void BindFlagAttribute<T>(this Element self, BindFlagAttributeOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            self.BindToggleAttribute(options);
        }

        /// <summary>
        /// Binds a flag attribute
        /// </summary>
        /// <typeparam name="T">Data type for data source instance</typeparam>
        /// <param name="self"></param>
        /// <param name="options">Binding options</param>
        [Obsolete("Use OnSourceChange instead")]
        public static void BindToggleAttribute<T>(this Element self, BindFlagAttributeOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            var source = options.BindObject ?? throw new ArgumentNullException(nameof(options.BindObject));
            var attribute = options.Attribute.ToLowerInvariant();
            var property = options.Property ?? throw new ArgumentNullException(nameof(options.Property));
            self.OnSourceChange(source, x => x.SetFlagAttributeLower(attribute, property(source)));
        }

        /// <summary>
        /// Bindings to toggle an element class
        /// </summary>
        /// <typeparam name="T">Data type for data source instance</typeparam>
        /// <param name="self"></param>
        /// <param name="options">Binding options</param>
        [Obsolete("Use OnSourceChange instead")]
        public static void BindToggleClass<T>(this Element self, BindToggleClassOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            var source = options.BindObject ?? throw new ArgumentNullException(nameof(options.BindObject));
            var property = options.Property ?? throw new ArgumentNullException(nameof(options.Property));
            var className = options.ClassName;
            if (string.IsNullOrWhiteSpace(className)) throw new ArgumentException("ClassName cannot be empty");
            self.OnSourceChange(source, x => x.ToggleClass(className, property(source)));
        }

        /// <summary>
        /// Two-way bindings for element attributes (e.g. 'value' attribute populated by user)
        /// </summary>
        /// <typeparam name="T">Source data type</typeparam>
        /// <param name="self"></param>
        /// <param name="options">Binding options</param>
        [Obsolete("Use OnSourceChange and OnChange instead")]
        public static void BindInput<T>(this Element self, BindInputOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            var source = options.BindObject
                ?? throw new ArgumentNullException(nameof(options.BindObject));
            var property = options.Property
                ?? throw new ArgumentNullException(nameof(options.Property));
            if (property.Body is not MemberExpression member)
            {
                throw new ArgumentException(Resources.InvalidBindingExpression);
            }
            var name = member.Member.Name;
            var attribute = options.Attribute;
            if (string.IsNullOrWhiteSpace(attribute))
            {
                throw new ArgumentException("Attribute cannot be empty");
            }
            attribute = attribute.ToLowerInvariant();
            var setter = CompileSetter(property);
            var getter = property.Compile();
            self.OnSourceChange(source, _ =>
            {
                var value = getter(source);
                self.SetAttributeLower(attribute, value);
            });
            self.OnChange(_ =>
            {
                var value = self.GetAttributeLower(attribute);
                setter(source, value);
            });
        }

        /// <summary>
        /// Two-way bindings for element flag attributes (e.g. 'checked' attribute populated by user)
        /// </summary>
        /// <typeparam name="T">Source data type</typeparam>
        /// <param name="self"></param>
        /// <param name="options">Binding options</param>
        [Obsolete("Use OnSourceChange and OnChange instead")]
        public static void BindFlagInput<T>(this Element self, BindFlagInputOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            var source = options.BindObject
                ?? throw new ArgumentNullException(nameof(options.BindObject));
            var property = options.Property
                ?? throw new ArgumentNullException(nameof(options.Property));
            if (property.Body is not MemberExpression member)
            {
                throw new ArgumentException(Resources.InvalidBindingExpression);
            }
            var name = member.Member.Name;
            var attribute = options.Attribute;
            if (string.IsNullOrWhiteSpace(attribute))
            {
                throw new ArgumentException("Attribute cannot be empty");
            }
            attribute = attribute.ToLowerInvariant();
            var setter = CompileSetter(property);
            var getter = property.Compile();
            self.OnSourceChange(source, _ =>
            {
                var value = getter(source);
                self.ToggleAttributeLower(attribute, value);
            });
            self.OnChange(_ =>
            {
                var value = self.HasAttributeLower(attribute);
                setter(source, value);
            });
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

        /// <summary>
        /// Removes bindings for an attribute
        /// </summary>
        /// <param name="self"></param>
        /// <param name="attribute">Attribute to remove bindings of</param>
        [Obsolete("Has no effect anymore. Use UnbindAll instead.")]
        public static void UnbindAttribute(this Element self, string attribute)
        {
        }

        /// <summary>
        /// Binds an element's inner text
        /// </summary>
        /// <typeparam name="T">Type of source data</typeparam>
        /// <param name="self"></param>
        /// <param name="options">Inner text binding options</param>
        [Obsolete("Use OnSourceChange instead")]
        public static void BindInnerText<T>(this Element self, BindInnerTextOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            var source = options.BindObject ?? throw new ArgumentNullException(nameof(options.BindObject));
            var property = options.Property ?? throw new ArgumentNullException(nameof(options.Property));
            self.OnSourceChange(source, x => x.InnerText = property(source));
        }

        /// <summary>
        /// Removes inner text bindings
        /// </summary>
        [Obsolete("Has no effect anymore. Use UnbindAll instead.")]
        public static void UnbindInnerText(this Element self)
        {
        }

        /// <summary>
        /// Removes bindings for the generic handler
        /// </summary>
        [Obsolete("Has no effect anymore. Use UnbindAll instead.")]
        public static void UnbindHandler(this Element self)
        {
        }

        /// <summary>
        /// Removes bindings for any attributes
        /// </summary>
        [Obsolete("Has no effect anymore. Use UnbindAll instead.")]
        public static void UnbindAttributes(this Element self)
        {
        }

        /// <summary>
        /// Binds the list of children to an observable collection
        /// </summary>
        /// <typeparam name="T">Type for items in the collection</typeparam>
        /// <param name="self"></param>
        /// <param name="options">Children binding options</param>
        [Obsolete("Use BindChildren(source, factory) instead")]
        public static void BindChildren<T>(this Element self, BindChildrenOptions<T> options)
            where T : class, INotifyPropertyChanged
        {
            self.BindChildren(options.Collection, options.CreateCallback);
        }

        /// <summary>
        /// Removes all bindings for the list of children
        /// </summary>
        [Obsolete("Has no effect anymore, use UnbindAll when needed")]
        public static void UnbindChildren(this Element self)
        {
        }

        /// <summary>
        /// Clears all child nodes and replaces them with a single text node
        /// </summary>
        /// <param name="self"></param>
        /// <param name="text">Text for the node</param>
        [Obsolete("Use InnerText property instead.")]
        public static void SetInnerText(this Element self, string text)
        {
            self.SetInnerEncode(text, true);
        }
    }
}
