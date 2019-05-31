/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

[assembly: InternalsVisibleTo("Tests")]
namespace Integrative.Lara.Tools
{
    static class LaraTools
    {
        static DataContractJsonSerializerSettings _jsonSettings;

        static LaraTools()
        {
            _jsonSettings = new DataContractJsonSerializerSettings
            {
                EmitTypeInformation = EmitTypeInformation.Never
            };
        }

        public static void LaunchBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")); // Works ok on windows
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);  // Works ok on linux
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url); // Not tested
            }
        }

        public static string Serialize<T>(T instance)
        {
            var stream = new MemoryStream();
            using (var reader = new StreamReader(stream))
            {
                var serializer = new DataContractJsonSerializer(typeof(T), _jsonSettings);
                serializer.WriteObject(stream, instance);
                stream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        public static T Deserialize<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return serializer.ReadObject(stream) as T;
            }
        }

        public static byte[] Compress(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            using (var stream = new DeflateStream(output, CompressionLevel.Fastest))
            {
                stream.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }
    }
}
