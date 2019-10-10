/*
Copyright (c) 2019 Integrative Software LLC
Created: 7/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Integrative.Lara.Tools
{
    static class AssembliesReader
    {
        public static void LoadAssemblies()
        {
            foreach (var assembly in GetAssembliesReferencingLara())
            {
                LoadAssembly(assembly);
            }
        }

        private static IEnumerable<Assembly> GetAssembliesReferencingLara()
        {
            var definedIn = typeof(LaraWebServiceAttribute).Assembly.GetName().Name;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (IncludeAssembly(assembly, definedIn))
                {
                    yield return assembly;
                }
            }
        }

        private static bool IncludeAssembly(Assembly assembly, string definedIn)
        {
            return (!assembly.GlobalAssemblyCache)
                && ((assembly.GetName().Name == definedIn)
                    || assembly.GetReferencedAssemblies().Any(a => a.Name == definedIn));
        }

        private static void LoadAssembly(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                LoadWebServices(type);
                LoadPages(type);
                LoadComponents(type);
            }
        }

        private static void LoadWebServices(Type type)
        {
            var services = type.GetCustomAttributes(typeof(LaraWebServiceAttribute), true);
            foreach (LaraWebServiceAttribute entry in services)
            {
                VerifyType(type, typeof(IWebService));
                LaraUI.Publish(new WebServiceContent
                {
                    Address = entry.Address,
                    ContentType = entry.ContentType,
                    Factory = () => (IWebService)Activator.CreateInstance(type),
                    Method = entry.Method
                });
            }
        }

        internal static void VerifyType(Type assemblyType, Type requiredType)
        {
            if (!requiredType.IsAssignableFrom(assemblyType))
            {
                var message = $"The class {assemblyType.FullName} marked as [LaraWebService] needs to implement/inherit [{requiredType.Name}].";
                throw new InvalidOperationException(message);
            }
        }

        private static void LoadPages(Type type)
        {
            var pages = type.GetCustomAttributes(typeof(LaraPageAttribute), true);
            foreach (LaraPageAttribute entry in pages)
            {
                VerifyType(type, typeof(IPage));
                LaraUI.Publish(entry.Address, () => (IPage)Activator.CreateInstance(type));
            }
        }

        private static void LoadComponents(Type type)
        {
            var components = type.GetCustomAttributes(typeof(LaraWebComponentAttribute), true);
            foreach (LaraWebComponentAttribute entry in components)
            {
                VerifyType(type, typeof(WebComponent));
                LaraUI.Publish(new WebComponentOptions
                {
                    ComponentTagName = entry.ComponentTagName,
                    ComponentType = type
                });
            }
        }
    }
}
