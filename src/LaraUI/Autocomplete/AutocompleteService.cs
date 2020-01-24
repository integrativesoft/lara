/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Integrative.Lara
{
    [DataContract]
    internal class AutocompleteRequest
    {
        [DataMember]
        public string Key { get; set; } = string.Empty;

        [DataMember]
        public string Term { get; set; } = string.Empty;
    }

    [LaraWebService(Address = Address)]
    internal class AutocompleteService : IWebService
    {
        private const string Address = "/lara_autocomplete";

        private static readonly AutocompleteRegistry _map = new AutocompleteRegistry();

        public Task<string> Execute()
        {
            return Execute(LaraUI.Service.RequestBody);
        }

        internal static async Task<string> Execute(string json)
        {
            var request = LaraUI.JSON.Parse<AutocompleteRequest>(json);
            if (!_map.TryGet(request.Key, out var element))
            {
                return string.Empty;
            }
            var options = element.GetOptions();
            if (options?.Provider == null)
            {
                return string.Empty;
            }
            else
            {
                var response = await options.Provider.GetAutocompleteList(request.Term);
                return LaraUI.JSON.Stringify(response);
            }
        }

        public static void Register(string key, AutocompleteElement element)
        {
            _map.Set(key, element);
        }

        public static void Unregister(string key)
        {
            _map.Remove(key);
        }

        public static int RegisteredCount => _map.Count;
    }
}
