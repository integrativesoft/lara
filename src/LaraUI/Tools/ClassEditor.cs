/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara.Front.Tools
{
    static class ClassEditor
    {
        public static string AddClass(string previous, string name)
        {
            if (HasClass(previous, name))
            {
                return previous;
            }
            else if (string.IsNullOrWhiteSpace(previous))
            {
                return name;
            }
            else
            {
                return previous.TrimEnd() + " " + name;
            }
        }

        public static string RemoveClass(string previous, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return previous;
            }
            name = name.Trim();
            previous = previous.Trim();
            if (previous == name)
            {
                return "";
            }
            else if (previous.StartsWith(name + " "))
            {
                return previous.Substring(name.Length + 1);
            }
            else if (previous.EndsWith(" " + name))
            {
                return previous.Substring(0, previous.Length - name.Length - 1);
            }
            int index = previous.IndexOf(" " + name + " ");
            if (index == -1)
            {
                return previous;
            }
            string left = previous.Substring(0, index);
            string right = previous.Substring(index + name.Length + 1);
            return left + right;
        }

        public static string ToggleClass(string previous, string name, bool value)
        {
            if (value)
            {
                return AddClass(previous, name);
            }
            else
            {
                return RemoveClass(previous, name);
            }
        }

        public static string ToggleClass(string previous, string name)
        {
            var value = HasClass(previous, name);
            return ToggleClass(previous, name, !value);
        }

        public static bool HasClass(string elementClass, string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                return true;
            }
            else if (string.IsNullOrWhiteSpace(elementClass))
            {
                return false;
            }
            else
            {
                elementClass = elementClass.Trim();
                return elementClass == className
                    || elementClass.StartsWith(className + " ")
                    || elementClass.EndsWith(" " + className)
                    || elementClass.Contains(" " + className + " ");
            }
        }

    }
}
