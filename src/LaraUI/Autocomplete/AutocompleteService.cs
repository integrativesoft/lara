/*
Copyright (c) 2019 Integrative Software LLC
Created: 11/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Integrative.Lara.Autocomplete
{
    [DataContract]
    class AutocompleteRequest
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Term { get; set; }
    }

    [LaraWebService(Address = Address)]
    class AutocompleteService : IWebService
    {
        public const string Address = "/lara_autocomplete";

        readonly static AutocompleteRegistry _map = new AutocompleteRegistry();

        public async Task<string> Execute()
        {
            var json = LaraUI.Service.RequestBody;
            var request = LaraUI.JSON.Parse<AutocompleteRequest>(json);
            if (!_map.TryGet(request.Key, out var element))
            {
                return string.Empty;
            }
            var options = element.GetOptions();
            var response = await options.Provider.GetAutocompleteList(request.Term);
            return LaraUI.JSON.Stringify(response);
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
