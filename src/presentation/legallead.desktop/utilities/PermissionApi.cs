using legallead.desktop.entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace legallead.desktop.utilities
{
    internal class PermissionApi
    {
        private readonly string _baseUri;

        public PermissionApi(string baseUri)
        {
            _baseUri = baseUri.TrimSlash();
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

        public async Task<ApiResponse> Post(string name, object payload, UserBo user)
        {
            var pageName = PostAddresses.Keys.FirstOrDefault(x => x.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(pageName)) { return new ApiResponse { Message = "Invalid page address." }; }
            if (!user.IsInitialized) { return new ApiResponse { Message = "Invalid user state. Please initialize user context." }; }
            var address = string.Format(PostAddresses[pageName], _baseUri);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("APP_IDENTITY", user.GetAppServiceHeader());
            var result = await client.PostAsJsonAsync(address, payload);
            if (result == null)
            {
                return new ApiResponse
                {
                    StatusCode = 500,
                    Message = "Unable to communicate with remote server"
                };
            }
            var content = await result.Content.ReadAsStringAsync();
            return new ApiResponse
            {
                StatusCode = (int)result.StatusCode,
                Message = content
            };
        }

        private static readonly Dictionary<string, string> GetAddresses = new()
        {
            { "application-read-me", "{0}/api/application/read-me" },
            { "application-list", "{0}/api/application/apps" }
        };

        private static readonly Dictionary<string, string> PostAddresses = new()
        {
            { "signon-login", "{0}/api/signon/login" },
            { "application-register", "{0}/api/Application/register" }
        };
    }
}