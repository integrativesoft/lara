/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Integrative.Lara
{
    internal sealed class ClientLibraryHandler : BaseHandler
    {
        private const string ResourceName = "Integrative.Lara.lara-client.js";

        private readonly string _address;
        private readonly byte[] _library;

        public ClientLibraryHandler(RequestDelegate next) : base(next)
        {
            var assembly = GetCurrentAssembly();
            var version = GetLibraryVersion(assembly);
            _address = BuildLibraryAddress(version);
            var js = LoadLibrary(assembly);
            _library = Encoding.UTF8.GetBytes(js);
        }

        private static Assembly GetCurrentAssembly()
        {
            return Assembly.GetAssembly(typeof(ClientLibraryHandler));
        }

        private static string GetLibraryVersion(Assembly assembly)
        {
            var info = FileVersionInfo.GetVersionInfo(assembly.Location);
            var version = info.FileVersion;
            return version;
        }

        private static string BuildLibraryAddress(string version)
        {
            version = version.Replace('.', '-');
            return GlobalConstants.LibraryAddress.Replace("{0}", version, StringComparison.InvariantCulture);
        }

        public static string GetLibraryPath()
        {
            var assembly = GetCurrentAssembly();
            var version = GetLibraryVersion(assembly);
            return BuildLibraryAddress(version);
        }

        private static string LoadLibrary(Assembly assembly)
        {
            using var stream = assembly.GetManifestResourceStream(ResourceName);
            if (stream == null) throw new InvalidOperationException(Resources.ResourceNotFound);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public static byte[] LoadFile(Assembly assembly, string name)
        {
            using var stream = assembly.GetManifestResourceStream(name);
            if (stream == null) throw new InvalidOperationException(Resources.ResourceNotFound);
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        internal override async Task<bool> ProcessRequest(HttpContext http)
        {
            if (http.Request.Method != "GET" || http.Request.Path != _address) return false;
            await SendLibrary(http);
            return true;

        }

        private async Task SendLibrary(HttpContext http)
        {
#if DEBUG
            MiddlewareCommon.AddHeaderPreventCaching(http);
#else
            MiddlewareCommon.AddHeaderNeverExpires(http);
#endif
            MiddlewareCommon.AddHeaderJSON(http);
            await MiddlewareCommon.WriteBuffer(http, _library);
        }
    }
}
