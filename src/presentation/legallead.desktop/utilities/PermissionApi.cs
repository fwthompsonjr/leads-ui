using legallead.desktop.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace legallead.desktop.utilities
{
    internal class PermissionApi
    {
        private readonly string _baseUri;

        public PermissionApi(string baseUri)
        {
            _baseUri = baseUri;
        }

        public async Task<ApiResponse> Get(string name)
        {
            var pageName = GetAddresses.Keys.FirstOrDefault(x => x.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(pageName)) { return new ApiResponse { Message = "Invalid page address." }; }
            var address = string.Format(GetAddresses[pageName], _baseUri);
            using var client = new HttpClient();
            var result = await client.GetStringAsync(address);
            if (result == null)
            {
                return new ApiResponse
                {
                    StatusCode = 500,
                    Message = "Unable to communicate with remote server"
                };
            }
            return new ApiResponse
            {
                StatusCode = 200,
                Message = result
            };
        }

        private static readonly Dictionary<string, string> GetAddresses = new()
        {
            { "application-read-me", "{0}/api/application/read-me" },
            { "application-list", "{0}/api/application/apps" }
        };
    }
}