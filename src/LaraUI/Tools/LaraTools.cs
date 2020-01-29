/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Json;
using System.Text;

[assembly: InternalsVisibleTo("Tests")]
namespace Integrative.Lara
{
    internal static class LaraTools
    {
        public static void LaunchBrowser(IWebHost host)
        {
            var address = GetFirstUrl(host);
            LaunchBrowser(address);
        }

        public static string GetFirstUrl(IWebHost webHost)
        {
            return webHost.ServerFeatures
                .Get<IServerAddressesFeature>()
                .Addresses
                .First();
        }

        public static void LaunchBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }

        public static string Serialize<T>(T instance)
        {
            return Serialize(instance, typeof(T));
        }

        public static string Serialize(object? instance, Type type)
        {
            if (instance == null)
            {
                return string.Empty;
            }
            var stream = new MemoryStream();
            using var reader = new StreamReader(stream);
            var serializer = new DataContractJsonSerializer(type);
            serializer.WriteObject(stream, instance);
            stream.Position = 0;
            return reader.ReadToEnd();
        }

        public static T? Deserialize<T>(string json) where T : class
        {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            return Deserialize<T>(stream);
        }

        public static T? Deserialize<T>(Stream stream) where T : class
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            return serializer.ReadObject(stream) as T;
        }

        public static byte[] Compress(byte[] data)
        {
            var output = new MemoryStream();
            using (var stream = new DeflateStream(output, CompressionLevel.Fastest))
            {
                stream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }

        public static bool SameValue<T>(T previous, T value)
        {
            if (previous == null)
            {
                return value == null;
            }
            else
            {
                return previous.Equals(value);
            }
        }
    }
}
