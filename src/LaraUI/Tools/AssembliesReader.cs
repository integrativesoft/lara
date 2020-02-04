/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 7/2019
Author: Pablo Carbonell
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Integrative.Lara
{
    internal static class AssembliesReader
    {
        public static void LoadAssemblies(Application app)
        {
            foreach (var assembly in GetAssembliesReferencingLara())
            {
                LoadAssembly(app, assembly);
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

        private static void LoadAssembly(Application app, Assembly assembly)
        {
            try
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    LoadWebServices(app, type);
                    LoadBinaryServices(app, type);
                    LoadPages(app, type);
                    LoadComponents(app, type);
                }
            }
            catch (ReflectionTypeLoadException)
            {
            }
        }

        private static void LoadWebServices(Application app, Type type)
        {
            var services = type.GetCustomAttributes(typeof(LaraWebServiceAttribute), true);
            foreach (LaraWebServiceAttribute entry in services)
            {
                VerifyType(type, "LaraWebService", typeof(IWebService));
                app.PublishService(new WebServiceContent
                {
                    Address = entry.Address,
                    ContentType = entry.ContentType,
                    Factory = () => (IWebService)Activator.CreateInstance(type),
                    Method = entry.Method
                });
            }
        }

        internal static void VerifyType(Type assemblyType, string attribute, Type requiredType)
        {
            if (requiredType.IsAssignableFrom(assemblyType)) return;
            var message = $"The class {assemblyType.FullName} marked as [{attribute}] needs to implement/inherit [{requiredType.Name}].";
            throw new InvalidOperationException(message);
        }

        private static void LoadBinaryServices(Application app, Type type)
        {
            var services = type.GetCustomAttributes(typeof(LaraBinaryServiceAttribute), true);
            foreach (LaraBinaryServiceAttribute entry in services)
            {
                VerifyType(type, "LaraBinaryService", typeof(IBinaryService));
                app.PublishService(new BinaryServiceContent
                {
                    Address = entry.Address,
                    ContentType = entry.ContentType,
                    Factory = () => (IBinaryService)Activator.CreateInstance(type),
                    Method = entry.Method
                });
            }
        }

        private static void LoadPages(Application app, Type type)
        {
            var pages = type.GetCustomAttributes(typeof(LaraPageAttribute), true);
            foreach (LaraPageAttribute entry in pages)
            {
                VerifyType(type, "LaraPage", typeof(IPage));
                app.PublishPage(entry.Address, () => (IPage)Activator.CreateInstance(type));
            }
        }

        private static void LoadComponents(Application app, Type type)
        {
            var components = type.GetCustomAttributes(typeof(LaraWebComponentAttribute), true);
            foreach (LaraWebComponentAttribute entry in components)
            {
                VerifyType(type, "LaraWebComponent", typeof(WebComponent));
                app.PublishComponent(new WebComponentOptions
                {
                    ComponentTagName = entry.ComponentTagName,
                    ComponentType = type
                });
            }
        }
    }
}
