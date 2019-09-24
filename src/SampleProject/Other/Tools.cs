/*
Copyright (c) 2019 Integrative Software LLC
Created: 9/2019
Author: Pablo Carbonell
*/

using System.Reflection;

namespace SampleProject.Other
{
    class Tools
    {
        public static byte[] LoadEmbeddedResource(Assembly assembly, string resourceName)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
