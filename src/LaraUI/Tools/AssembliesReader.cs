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
            var definedIn = typeof(LaraWebService).Assembly.GetName().Name;
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
            var services = type.GetCustomAttributes(typeof(LaraWebService), true);
            foreach (LaraWebService entry in services)
            {
                if (!typeof(IWebService).IsAssignableFrom(type))
                {
                    var message = $"The class {type.FullName} marked as [LaraWebService] needs to implement the interface IWebService.";
                    throw new InvalidOperationException(message);
                }
                LaraUI.Publish(new WebServiceContent
                {
                    Address = entry.Address,
                    ContentType = entry.ContentType,
                    Factory = () => (IWebService)Activator.CreateInstance(type),
                    Method = entry.Method
                });
            }
        }

        private static void LoadPages(Type type)
        {
            var pages = type.GetCustomAttributes(typeof(LaraPage), true);
            foreach (LaraPage entry in pages)
            {
                if (!typeof(IPage).IsAssignableFrom(type))
                {
                    var message = $"The class {type.FullName} marked as [LaraPge] needs to implement the interface IPage.";
                    throw new InvalidOperationException(message);
                }
                LaraUI.Publish(entry.Address, () => (IPage)Activator.CreateInstance(type));
            }
        }

        private static void LoadComponents(Type type)
        {
            var components = type.GetCustomAttributes(typeof(LaraWebComponent), true);
            foreach (LaraWebComponent entry in components)
            {
                if (!typeof(WebComponent).IsAssignableFrom(type))
                {
                    var message = $"The class {type.FullName} marked as [LaraWebComponent] needs to inherit from WebComponent.";
                    throw new InvalidOperationException(message);
                }
                LaraUI.Publish(new WebComponentOptions
                {
                    ComponentTagName = entry.ComponentTagName,
                    ComponentType = type
                });
            }
        }
    }
}
