using CefSharp;
using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace legallead.desktop.handlers
{
    internal class JsHomeFormSubmittedHandler : JsCompletedHandler
    {
        public JsHomeFormSubmittedHandler(ChromiumWebBrowser? browser) : base(browser)
        {
        }

        public override void Complete(Window window, Dispatcher dispatcher, ContentControl control, string? customData = null)
        {
        }

        public override void Submit(string formName, string json)
        {
            var handler = new LoginHandler(Web, formName, json);
            if (!handler.IsValid)
            {
                return;
            }
            try
            {
                handler.Start();
                var objectData = ConvertTo(formName, json).Result;
                var htm = ConvertHTML(objectData);
                handler.SetMessage(htm);
            }
            catch (Exception ex)
            {
                var exresponse = new ApiResponse { StatusCode = 500, Message = ex.Message };
                var exmsg = ConvertHTML(exresponse);
                handler.SetMessage(exmsg);
            }
            finally
            {
                handler.End();
            }
        }

        private static string ConvertHTML(ApiResponse response)
        {
            const int statusOk = 200;
            var code = response.StatusCode;
            var statusClass = code == statusOk ? "text-success" : "text-danger";
            var borderCode = code == statusOk ? "border-secordary" : "border-danger";
            var builder = new StringBuilder($"<div class='border {borderCode} m-2 p-2 fs-5cd'>");
            var message = string.IsNullOrWhiteSpace(response.Message) ? "No data provided" : response.Message;
            builder.AppendLine();
            builder.AppendLine($"\t<span name='status-code' class='{statusClass}'> {code:D3} Status </span> <br/>");
            builder.AppendLine($"\t<span name='status-message' class='text-secondary'> {message} </span> <br/>");
            builder.AppendLine("</div>");
            return builder.ToString();
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
                    var obj = new { data.UserName, data.Password };
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

        private sealed class LoginHandler
        {
            private readonly ChromiumWebBrowser? _web;
            public int FormIndex { get; private set; }
            public string Payload { get; private set; }
            public bool IsValid => FormIndex >= 0;
            private readonly bool? HasWebBrowser;

            public LoginHandler(ChromiumWebBrowser? browser, string formName, string json)
            {
                FormIndex = formNames.FindIndex(x => x.Equals(formName, StringComparison.OrdinalIgnoreCase));
                Payload = json;
                _web = browser;

                HasWebBrowser = IsValid && _web != null && _web.CanExecuteJavascriptInMainFrame;
            }

            public void Start()
            {
                if (!HasWebBrowser.GetValueOrDefault()) return;

                scriptNames.ForEach(s =>
                {
                    if (scriptNames.IndexOf(s) == 0)
                    {
                        // setup animation so user knows process is running
                        _web.ExecuteScriptAsync(s, FormIndex, true);
                    }
                    if (scriptNames.IndexOf(s) == 1)
                    {
                        // clear any previous submission messages
                        _web.ExecuteScriptAsync(s, FormIndex, "", false);
                    }
                });
            }

            internal void SetMessage(string htm)
            {
                if (!HasWebBrowser.GetValueOrDefault()) return;
                _web.ExecuteScriptAsync(scriptNames[1], FormIndex, htm, true);
            }

            public void End()
            {
                if (!HasWebBrowser.GetValueOrDefault()) return;
                _web.ExecuteScriptAsync(scriptNames[0], FormIndex, false);
            }

            private static readonly List<string> scriptNames = new()
            {
                "setIconState",
                "setStatusMessage"
            };
        }
    }
}