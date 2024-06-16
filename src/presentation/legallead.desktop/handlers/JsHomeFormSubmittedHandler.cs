using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.extensions;
using legallead.desktop.interfaces;
using legallead.desktop.models;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
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
            var handler = new LoginFormHandler(Web, formName, json);
            if (!handler.IsValid)
            {
                return;
            }
            try
            {
                handler.Start();
                var objectData = ConvertTo(formName, json, this).Result;
                var htm = ConvertHTML(objectData);
                handler.SetMessage(htm);
                if (objectData.StatusCode != 200) return;
                handler.SetMessage("");
                handler.ClearPassword();
                handler.LoginCompleted(formName);
                if (formName == "form-register")
                {

                    Window mainWindow = Application.Current.Dispatcher.Invoke(() =>
                    {
                        return Application.Current.MainWindow;
                    });
                    if (mainWindow is MainWindow main)
                    {
                        main.NavigateToMyAccount();
                        return;
                    }
                    return;
                }
                var data = TryDeserialize<LoginFormModel>(json);
                SetUserSession(data, Guid.NewGuid().ToString());
                NavigateTo("MyAccount", objectData);
                Thread.Sleep(500);

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

        private static void SetUserSession(LoginFormModel? login, string sessionId)
        {
            var bo = AppBuilder.ServiceProvider?.GetService<UserBo>();
            var user = AppBuilder.ServiceProvider?.GetService<UserSearchBo>();
            if (login == null || user == null || bo == null) return;
            bo.UserName = login.UserName;
            user.UserName = login.UserName;
            user.SessionId = sessionId;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar Qube",
            "S2589:Boolean expressions should not be gratuitous",
            Justification = "False positive on Line 100. Null check is valid.")]
        private static void NavigateTo(string destination, ApiResponse objectData)
        {
            var provider = AppBuilder.ServiceProvider;
            var user = provider?.GetRequiredService<UserBo>();
            var dispatcher = Application.Current.Dispatcher;
            Window mainWindow = dispatcher.Invoke(() => { return Application.Current.MainWindow; });
            if (mainWindow is not MainWindow main) return;
            if (user == null) return;
            var token = ObjectExtensions.TryGet<AccessTokenBo>(objectData.Message);
            user.Token = token;
            if (provider != null && token != null)
            {
                var api = provider.GetService<IPermissionApi>();
                user.SetUserId(api).GetAwaiter().GetResult();
            }
            main.NavigateTo(destination);
            main.NavigateToMySearch();
        }

        private static async Task<ApiResponse> ConvertTo(string formName, string json, JsHomeFormSubmittedHandler? handler = null)
        {
            const string failureMessage = "Unable to parse form submission data.";
            var failed = new ApiResponse { StatusCode = 402, Message = failureMessage };
            var succeeded = new ApiResponse { StatusCode = 200, Message = "Form processed as expected!" };
            var matchedName = HomeFormNames.Find(x => x.Equals(formName, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(matchedName))
            {
                return failed;
            }

            var provider = AppBuilder.ServiceProvider;
            if (provider == null) return failed;

            var api = provider.GetRequiredService<IPermissionApi>();
            if (api == null) return failed;

            var user = provider.GetRequiredService<UserBo>();
            if (user == null) return failed;

            switch (matchedName)
            {
                case "form-login":
                    var data = TryDeserialize<LoginFormModel>(json);
                    if (data == null) return failed;
                    var obj = new { data.UserName, data.Password };
                    var loginResponse = await api.Post("login", obj, user) ?? failed;
                    return loginResponse;

                case "form-register":
                    var data1 = TryDeserialize<UserRegistrationModel>(json);
                    if (data1 == null) return failed;
                    var obj1 = data1.ToApiModel();
                    var registerResponse = await api.Post("register", obj1, user) ?? failed;
                    if (!RegistrationCompletedHandler(data1, registerResponse, handler)) return failed;
                    return succeeded;

                default:
                    return succeeded;
            }
        }

        internal static readonly List<string> HomeFormNames = new() { "form-login", "form-register" };

        private static bool RegistrationCompletedHandler(UserRegistrationModel model, ApiResponse registration, JsHomeFormSubmittedHandler? handler = null)
        {
            if (handler == null) return false;
            if (registration.StatusCode != 200) return false;
            var json = $"'username': '{model.UserName}', 'login-password': '{model.Password}'";
            json = string.Concat("{ ", json, " }");
            try
            {
                handler.Submit("form-login", json);
                return true;
            }
            catch
            {
                return false;
            }

        }

    }
}