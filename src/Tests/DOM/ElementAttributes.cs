/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Reflection;
using Xunit;

namespace Integrative.Lara.Tests.DOM
{
    public class ElementAttributes
    {
        int _counter;

        [Fact]
        public void ElementProperties()
        {
            TestElement<Anchor>("a");
            TestElement<Button>("button");
            TestElement<ColGroup>("colgroup");
            TestElement<Image>("img");
            TestElement<Input>("input");
            TestElement<Label>("label");
            TestElement<Link>("link");
            TestElement<ListItem>("li");
            TestElement<Meta>("meta");
            TestElement<Meter>("meter");
            TestElement<Option>("option");
            TestElement<OptionGroup>("optgroup");
            TestElement<OrderedList>("ol");
            TestElement<Script>("script");
            TestElement<Select>("select");
            TestElement<Table>("table");
            TestElement<TableCell>("td");
            TestElement<TableHeader>("th");
            TestElement<TextArea>("textarea");
        }

        [Fact]
        public void ElementNeedsTag()
        {
            DomOperationsTesting.Throws<ArgumentNullException>(() => Element.Create(""));
            DomOperationsTesting.Throws<ArgumentNullException>(() => Element.Create(null));
        }

        private void TestElement<T>(string tagName) where T : Element
        {
            var instance = (Element)Activator.CreateInstance(typeof(T));
            Assert.Equal(tagName, instance.TagName);
            TestProperties(instance);
        }

        private void TestProperties(Element instance)
        {
            var type = instance.GetType();
            foreach (var property in type.GetProperties())
            {
                if (property.SetMethod != null)
                {
                    TestProperty(instance, property);
                }
            }
        }

        private void TestProperty(Element instance, PropertyInfo property)
        {
            var type = property.PropertyType;
            if (!GetTestValue(type, out var value))
            {
                return;
            }
            property.SetValue(instance, value);
            var result = property.GetValue(instance);
            Assert.Equal(value, result);
        }

        private bool GetTestValue(Type type, out object value)
        {
            _counter++;
            if (type == typeof(bool))
            {
                value = true;
            }
            else if (type == typeof(int))
            {
                value = _counter;
            }
            else if (type == typeof(string))
            {
                value = _counter.ToString();
            }
            else
            {
                value = null;
                return false;
            }
            return true;
        }
    }
}
