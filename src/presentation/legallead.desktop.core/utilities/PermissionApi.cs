using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Runtime.Caching;

namespace legallead.desktop.utilities
{
    internal class PermissionApi : IPermissionApi
    {
        protected readonly string _baseUri;
        protected readonly IInternetStatus? _connectionStatus;

        public PermissionApi(string baseUri)
        {
            _baseUri = baseUri.TrimSlash();
            var provider = DesktopCoreServiceProvider.Provider;
            if (provider == null) return;
            _connectionStatus ??= provider.GetService<IInternetStatus>();
        }

        public PermissionApi(string baseUri, IInternetStatus status) : this(baseUri)
        {
            _connectionStatus = status;
        }

        public IInternetStatus? InternetUtility => _connectionStatus;

        public ApiResponse CheckAddress(string name)
        {
            if (string.IsNullOrEmpty(_baseUri))
            {
                return new ApiResponse { StatusCode = 503, Message = "Base api address is missing or not defined." };
            }
            var pageName = GetAddresses.Keys.FirstOrDefault(x => x.EndsWith(name, StringComparison.OrdinalIgnoreCase));
            if (pageName == null)
            {
                return new ApiResponse { StatusCode = 404, Message = "Invalid page address." };
            }
            if (!GetConnectionStatus(name, pageName))
            {
                return new ApiResponse { StatusCode = 503, Message = "Page is not available." };
            }
            return new ApiResponse { StatusCode = 200, Message = pageName };
        }

        public KeyValuePair<bool, ApiResponse> CanGet(string name)
        {
            var internetOn = InternetUtility?.GetConnectionStatus() ?? true;
            if (!internetOn)
            {
                return new KeyValuePair<bool, ApiResponse>(false, nointernet);
            }
            var addressCheck = CheckAddress(name);
            if (addressCheck.StatusCode != 200) return new KeyValuePair<bool, ApiResponse>(false, addressCheck);
            return new KeyValuePair<bool, ApiResponse>(true, addressCheck);
        }

        public KeyValuePair<bool, ApiResponse> CanPost(string name, object payload, UserBo user)
        {
            var canget = CanGet(name);
            if (!canget.Key) return new KeyValuePair<bool, ApiResponse>(false, canget.Value);
            if (!user.IsInitialized)
            {
                // do somthing to invalidate or fix
                var current = canget.Value;
                current.StatusCode = 500;
                current.Message = "User account is not initialized. Please check application settings";
                return new KeyValuePair<bool, ApiResponse>(false, current);
            }
            return new KeyValuePair<bool, ApiResponse>(true, canget.Value);
        }

        public virtual async Task<ApiResponse> Get(string name)
        {
            try
            {
                var response = await Task.Run(() =>
                {
                    var verify = CanGet(name);
                    if (!verify.Key) return verify.Value;
                    return new ApiResponse
                    {
                        StatusCode = 200,
                        Message = "API call is to be executed from derived class."
                    };
                });
                return response;
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    StatusCode = 500,
                    Message = ex.Message
                };
            }
        }

        public virtual async Task<ApiResponse> Post(string name, object payload, UserBo user)
        {
            var canpost = CanPost(name, payload, user);
            if (!canpost.Key) return canpost.Value;

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

        protected virtual bool GetConnectionStatus(string name, string address)
        {
            return true;
        }

        protected static readonly Dictionary<string, string> GetAddresses = new()
        {
            { "application-read-me", "{0}/api/application/read-me" },
            { "application-list", "{0}/api/application/apps" }
        };

        protected static readonly Dictionary<string, string> PostAddresses = new()
        {
            { "signon-login", "{0}/api/signon/login" },
            { "application-register", "{0}/api/Application/register" }
        };

        protected static bool CanConnectedToPage(string address)
        {
            bool result = false;
            Ping p = new();
            try
            {
                PingReply reply = p.Send(address, 3000);
                if (reply.Status == IPStatus.Success)
                    return true;
            }
            catch
            {
                return result;
            }
            return result;
        }

        protected const string PageKeyName = "page-{0}-status";

        private static readonly ApiResponse nointernet = new() { StatusCode = 503, Message = "Application is unable to connect to internet." };
    }
}