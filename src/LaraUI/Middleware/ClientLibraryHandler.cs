/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using Integrative.Lara.Main;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Integrative.Lara.Middleware
{
    sealed class ClientLibraryHandler : BaseHandler
    {
        const string ResourceName = "Integrative.Lara.LaraClient.js";

        readonly string _address;
        readonly byte[] _library;

        public ClientLibraryHandler(RequestDelegate next) : base(next)
        {
            Assembly assembly = GetCurrentAssembly();
            string version = GetLibraryVersion(assembly);
            _address = BuildLibraryAddress(version);
            string js = LoadLibrary(assembly);
            _library = Encoding.UTF8.GetBytes(js);
        }

        private static Assembly GetCurrentAssembly()
        {
            return Assembly.GetAssembly(typeof(ClientLibraryHandler));
        }

        private static string GetLibraryVersion(Assembly assembly)
        {
            var info = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = info.FileVersion;
            return version;
        }

        private static string BuildLibraryAddress(string version)
        {
            return string.Format(GlobalConstants.LibraryAddress, version.Replace('.', '-'));
        }

        public static string GetLibraryPath()
        {
            var assembly = GetCurrentAssembly();
            var version = GetLibraryVersion(assembly);
            return BuildLibraryAddress(version);
        }

        private static string LoadLibrary(Assembly assembly)
        {
            using (var stream = assembly.GetManifestResourceStream(ResourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static byte[] LoadFile(Assembly assembly, string name)
        {
            using (var stream = assembly.GetManifestResourceStream(name))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return bytes;
            }
        }

        internal override async Task<bool> ProcessRequest(HttpContext http)
        {
            if (http.Request.Method == "GET"
                && http.Request.Path == _address)
            {
                await SendLibrary(http);
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task SendLibrary(HttpContext http)
        {
            MiddlewareCommon.SetStatusCode(http, HttpStatusCode.OK);
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
