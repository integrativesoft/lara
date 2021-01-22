/*
Copyright (c) 2019-2021 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    internal static class ClassEditor
    {
        public static string? AddClass(string? previous, string name)
        {
            if (HasClass(previous, name))
            {
                return previous;
            }

            if (string.IsNullOrWhiteSpace(previous))
            {
                return name;
            }
            return previous.TrimEnd() + " " + name;
        }

        public static string? RemoveClass(string? previous, string name)
        {
            if (string.IsNullOrWhiteSpace(name)
                || string.IsNullOrWhiteSpace(previous))
            {
                return previous;
            }
            name = name.Trim();
            previous = previous.Trim();
            if (previous == name)
            {
                return "";
            }

            if (previous.StartsWith(name + " ", System.StringComparison.InvariantCulture))
            {
                return previous.Substring(name.Length + 1);
            }
            if (previous.EndsWith(" " + name, System.StringComparison.InvariantCulture))
            {
                return previous.Substring(0, previous.Length - name.Length - 1);
            }

            var index = previous.IndexOf(" " + name + " ", System.StringComparison.InvariantCulture);
            if (index == -1)
            {
                return previous;
            }
            var left = previous.Substring(0, index);
            var right = previous.Substring(index + name.Length + 1);
            return left + right;
        }

        public static string? ToggleClass(string? previous, string name, bool value)
        {
            return value ? AddClass(previous, name) : RemoveClass(previous, name);
        }

        public static string? ToggleClass(string? previous, string name)
        {
            var value = HasClass(previous, name);
            return ToggleClass(previous, name, !value);
        }

        public static bool HasClass(string? elementClass, string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(elementClass))
            {
                return false;
            }
            elementClass = elementClass.Trim();
            return elementClass == className
                   || elementClass.StartsWith(className + " ", System.StringComparison.InvariantCulture)
                   || elementClass.EndsWith(" " + className, System.StringComparison.InvariantCulture)
                   || elementClass.Contains(" " + className + " ", System.StringComparison.InvariantCulture);
        }

    }
}
