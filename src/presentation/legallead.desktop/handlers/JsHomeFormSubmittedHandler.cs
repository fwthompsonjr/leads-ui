using legallead.desktop.entities;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace legallead.desktop.handlers
{
    internal class JsHomeFormSubmittedHandler : JsCompletedHandler
    {
        public override void Complete(Window window, Dispatcher dispatcher, ContentControl control, string? customData = null)
        {
        }

        public override string Submit(string formName, string json)
        {
            if (!formNames.Exists(x => x.Equals(formName, StringComparison.OrdinalIgnoreCase)))
            {
                var response = new ApiResponse { StatusCode = 402, Message = "Invalid form name provided." };
                return JsonConvert.SerializeObject(response);
            }
            var objectData = ConvertTo(formName, json).Result;
            return JsonConvert.SerializeObject(objectData);
        }

        private static async Task<ApiResponse> ConvertTo(string formName, string json)
        {
            const string failureMessage = "Unable to parse form submission data.";
            var failed = new ApiResponse { StatusCode = 402, Message = failureMessage };
            var succeeded = new ApiResponse { StatusCode = 200, Message = "Form processed as expected!" };
            var matchedName = formNames.Find(x => x.Equals(formName, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(matchedName))
            {
                return failed;
            }

            var provider = AppBuilder.ServiceProvider;
            if (provider == null) return failed;

            var api = provider.GetRequiredService<PermissionApi>();
            if (api == null) return failed;

            var user = provider.GetRequiredService<UserBo>();
            if (user == null) return failed;

            switch (matchedName)
            {
                case "form-login":
                    var data = TryDeserialize<LoginForm>(json);
                    if (data == null) return failed;
                    var obj = JsonConvert.SerializeObject(new { data.UserName, data.Password }) ?? string.Empty;
                    var loginResponse = await api.Post("login", obj, user) ?? failed;
                    return loginResponse;

                default:
                    return succeeded;
            }
        }

        private static readonly List<string> formNames = new() { "form-login", "form-register" };

        private sealed class LoginForm
        {
            [JsonProperty("username")]
            public string UserName { get; set; } = string.Empty;

            [JsonProperty("login-password")]
            public string Password { get; set; } = string.Empty;
        }
    }
}