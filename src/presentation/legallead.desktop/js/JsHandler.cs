using CefSharp;
using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.models;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace legallead.desktop.js
{
    internal class JsHandler
    {
        protected readonly ChromiumWebBrowser? web;

        public JsHandler(ChromiumWebBrowser? browser)
        {
            web = browser;
            AppBuilder.Build();
        }

        public virtual void OnPageLoaded()
        {
        }

        public virtual void Reload(string pageName)
        {
            var dispatcher = Application.Current.Dispatcher;
            Window mainWindow = dispatcher.Invoke(() => { return Application.Current.MainWindow; });
            if (mainWindow is not MainWindow main) return;
            main.NavigateChild(pageName);
        }
        public virtual void Populate()
        {
            var provider = AppBuilder.ServiceProvider;
            var user = provider?.GetService<UserBo>();
            var api = provider?.GetService<IPermissionApi>();
            if (user == null || !user.IsAuthenicated)
            {
                Reload("home-login");
                return;
            }
            if (api == null) return;

            var payload = new { id = Guid.NewGuid().ToString(), name = "legallead.permissions.api" };
            var appresponse = api.Post("search-get-actives", payload, user).Result;
            if (appresponse == null || appresponse.StatusCode != 200) return;
            if (web == null) return;
            var message = JsSearchSubmissionHelper.SortHistory(appresponse.Message);
            web.ExecuteScriptAsync("injectJson", message);
        }
        public virtual void CheckSession()
        {
            var bo = AppBuilder.ServiceProvider?.GetService<UserBo>();
            if (bo == null || bo.IsSessionExpired())
            {
                Reload("home-login");
            }
        }

        public virtual async void Reauthenticate(string payload)
        {
            var data = TryDeserialize<LoginFormModel>(payload);
            var api = AppBuilder.ServiceProvider?.GetService<IPermissionApi>();
            var bo = AppBuilder.ServiceProvider?.GetService<UserBo>();
            if (data == null || api == null || bo == null)
            {
                Reload("home-login");
                return;
            }
            var obj = new { data.UserName, data.Password };
            var response = await api.Post("form-login", obj, bo);
            if (response == null || response.StatusCode != 200)
            {
                Reload("home-login");
                return;
            }
            // update access token 
            bo.Token = ObjectExtensions.TryGet<AccessTokenBo>(response.Message);
            if (bo.Token == null)
            {
                Reload("home-login");
                return;
            }
            // dismiss the dialog and clear its values
            var scripts = new[]
            {
                "try {",
                "document.getElementById('account-authorize-x-close').click();",
                "document.getElementById('form-re-authorize-username').value = '';",
                "document.getElementById('form-re-authorize-password').value = '';",
                "} catch { }"
            };
            var script = string.Join(Environment.NewLine, scripts);
            web.ExecuteScriptAsync(script);
        }

        public virtual void Fetch(string formName, string json)
        {
            if (!"frm-search-make-payment".Equals(formName)) return;
            var payload = JsonConvert.DeserializeObject<GenerateInvoiceModel>(json) ?? new();
            var api = AppBuilder.ServiceProvider?.GetService<IPermissionApi>();
            if (api == null || string.IsNullOrEmpty(payload.Id)) return;
            var response = api.Get(
                "user-zero-payment",
                new Dictionary<string, string>() { { "~0", payload.Id } }).Result;
            if (response == null || response.StatusCode != 200) return;
            var dispatcher = Application.Current.Dispatcher;
            web?.SetHTML(dispatcher, response.Message);
        }

        public virtual void Submit(string formName, string json)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            var isProfileForm = ProfileForms.Exists(f => f.Equals(formName, comparison));
            var isPermissionForm = PermissionForms.Exists(f => f.Equals(formName, comparison));
            var isSearchForm = SearchForms.Exists(f => f.Equals(formName, comparison));
            var isSessionCheckNeeded = isProfileForm || isPermissionForm || isSearchForm;
            if (isSessionCheckNeeded && IsSessionTimeout(web))
            {
                const string script1 = "document.getElementById('form-re-authorize-username').value = '~0'";
                const string script2 = "document.getElementById('btn-account-authorize-show').click();";
                var user = AppBuilder.ServiceProvider?.GetService<UserBo>();
                var userId = user?.UserName ?? string.Empty;
                var arr = new[] { script1.Replace("~0", userId), script2 };
                var script = string.Join(Environment.NewLine, arr);
                web?.ExecuteScriptAsync(script);
                return;
            }
            if (isProfileForm)
            {
                var handler = new JsProfileChange(web);
                handler.Submit(formName, json);
            }
            if (isPermissionForm)
            {
                var handler = new JsPermissionChange(web);
                handler.Submit(formName, json);
            }
            if (isSearchForm)
            {
                var handler = new JsSearchSubmission(web);
                handler.Submit(formName, json);
            }
        }
        public virtual async void GetPurchases()
        {
            var user = AppBuilder.ServiceProvider?.GetService<UserBo>();
            var api = AppBuilder.ServiceProvider?.GetService<IPermissionApi>();
            if (user == null || api == null || web == null) return;
            var json = await GetPurchasesAsync(user, api);
            if (string.IsNullOrWhiteSpace(json)) return;
            web.ExecuteScriptAsync("jsPurchases.bind_purchase", json);
        }

        public virtual async void MakeDownload(string json)
        {
            const string openFileSpan = "<span onclick='jsPurchases.open_file()' id='user-download-file-name' style='cursor:pointer' class='text-white'>";
            var user = AppBuilder.ServiceProvider?.GetService<UserBo>();
            var api = AppBuilder.ServiceProvider?.GetService<IPermissionApi>();
            var dispatcher = Application.Current.Dispatcher;
            var main = dispatcher.Invoke(() => { return Application.Current.MainWindow; });
            var window = (MainWindow)main;
            if (user == null
                || api == null
                || web == null
                || window == null
                || string.IsNullOrWhiteSpace(json))
            {
                var mssg = DownloadStatusMessaging.GetMessage(400, "One or more expected dependencies are missing or invalid.");
                web.ExecuteScriptAsync("jsPurchases.show_submission_error", mssg);
                return;
            }
            var payload = DownloadJson.FromJson(json);
            var dirName = dispatcher.Invoke(() =>
            {
                return window.ShowFolderBrowserDialog();
            });
            if (payload != null) { payload.Name = dirName; }
            if (payload == null || !payload.IsValid)
            {
                var mssg = DownloadStatusMessaging.GetMessage(422, "One or more expected form values are incorrect.");
                web.ExecuteScriptAsync("jsPurchases.show_submission_error", mssg);
                return;
            }
            var adjustedName = payload.CalculateFileName();
            if (!TestCreationForTmp(adjustedName))
            {
                var mssg = DownloadStatusMessaging.GetMessage(206, "Unable to write file content to your desktop location.");
                web.ExecuteScriptAsync("jsPurchases.show_submission_error", mssg);
            }
            var response = await api.Post("make-search-purchase", payload, user);
            if (response == null)
            {
                var mssg = DownloadStatusMessaging.GetMessage(500, "An error occurred processing your request.");
                web.ExecuteScriptAsync("jsPurchases.show_submission_error", mssg);
                return;
            }
            if (response.StatusCode != 200)
            {
                var mssg = DownloadStatusMessaging.GetMessage(response.StatusCode, response.Message);
                web.ExecuteScriptAsync("jsPurchases.show_submission_error", mssg);
                return;
            }
            var deserialized = TryDeserialize<DownloadJsResponse>(response.Message);
            if (deserialized == null ||
                !string.IsNullOrEmpty(deserialized.Error) ||
                string.IsNullOrEmpty(deserialized.Content))
            {
                var explained = deserialized?.Error ?? "An error occurred processing your request";
                var mssg = DownloadStatusMessaging.GetMessage(500, explained);
                web.ExecuteScriptAsync("jsPurchases.show_submission_error", mssg);
                return;
            }
            var content = GetBytes(deserialized.Content);
            if (content == null)
            {
                var mssg = DownloadStatusMessaging.GetMessage(500, "Unable to retrieve file content from server.");
                web.ExecuteScriptAsync("jsPurchases.show_submission_error", mssg);
                return;
            }

            var isCreated = TryCreateFile(content, adjustedName);

            if (!isCreated)
            {
                var mssg = DownloadStatusMessaging.GetMessage(206, "Unable to write file content to your desktop location.");
                web.ExecuteScriptAsync("jsPurchases.show_submission_error", mssg);
                var resetObj = new { UserId = user.UserName, ExternalId = payload.Id };
                await api.Post("reset-download", resetObj, user);
                return;
            }
            var msg = "Status code: 200<br/>"
                    + Environment.NewLine +
                    "File created successfully<br/>"
                    + Environment.NewLine +
                    $"Please open file at : <br/>{openFileSpan}{adjustedName}</span>";
            web.ExecuteScriptAsync("jsPurchases.show_submission_success", msg);
            AppBuilder.HistoryService?.OnTimer(null);
        }

        public virtual void LogoutUser()
        {
            var user = AppBuilder.ServiceProvider?.GetRequiredService<UserBo>();
            if (user == null) return;
            user.Token = null;
        }

        /// <summary>
        /// This callback function is not working, possibly due to injected content.
        /// Will revist after other application function have been completed
        /// </summary>
        /// <param name="sourceFile"></param>
        public virtual void TryOpenExcel(string sourceFile)
        {
            if (string.IsNullOrEmpty(sourceFile))
            {
                var mssg = DownloadStatusMessaging.GetMessage(400, "File name is invalid");
                web.ExecuteScriptAsync("jsPurchases.show_submission_error", mssg);
                return;
            }
            if (!File.Exists(sourceFile))
            {
                var mssg = DownloadStatusMessaging.GetMessage(400, $"File name = {Path.GetFileName(sourceFile)} not found");
                web.ExecuteScriptAsync("jsPurchases.show_submission_error", mssg);
                return;
            }
            try
            {
                Process excel = new();
                excel.StartInfo.FileName = @"C:\Test.xlsx";
                excel.Start();
            }
            catch (Exception ex)
            {

                var mssg = DownloadStatusMessaging.GetMessage(400, ex.Message);
                web.ExecuteScriptAsync("jsPurchases.show_submission_error", mssg);
            }
        }

        public virtual Action<object?>? OnInitCompleted { get; set; }

        protected static void Init()
        {
            var provider = AppBuilder.ServiceProvider;
            if (provider == null) return;

            var api = provider.GetRequiredService<IPermissionApi>();
            var user = provider.GetRequiredService<UserBo>();
            if (api == null || user == null || user.IsInitialized) return;
            var list = api.Get("list").Result;
            if (list == null || list.StatusCode != 200 || string.IsNullOrEmpty(list.Message)) return;
            var applications = TryDeserialize<ApiContext[]>(list.Message);
            user.Applications = applications;
        }

        protected static bool IsSessionTimeout(ChromiumWebBrowser? web)
        {
            if (web == null) return false;
            var user = AppBuilder.ServiceProvider?.GetService<UserBo>();
            if (user == null) return false;
            return user.IsSessionTimeout();
        }

        protected static T? TryDeserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default;
            }
        }

        protected static readonly List<string> ProfileForms = new()
        {
            "frm-profile-personal",
            "frm-profile-address",
            "frm-profile-phone",
            "frm-profile-email"
        };

        protected static readonly List<string> PermissionForms = new()
        {
            "permissions-subscription-group",
            "permissions-discounts",
            "form-change-password"
        };

        protected static readonly List<string> SearchForms = new()
        {
            "frm-search",
            "frm-search-history",
            "frm-search-purchases",
            "frm-search-preview",
            "frm-search-invoice"
        };

        private static async Task<string> GetPurchasesAsync(UserBo user, IPermissionApi api)
        {
            var parms = new Dictionary<string, string>
            {
                { "~0", user.UserName }
            };
            var response = await api.Get("user-purchase-history", user, parms);
            if (response == null) return string.Empty;
            if (response.StatusCode != 200) return string.Empty;
            return response.Message ?? string.Empty;

        }

        private static byte[]? GetBytes(string source)
        {
            try
            {
                return Convert.FromBase64String(source);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool TryCreateFile(byte[] data, string fileName)
        {
            try
            {
                if (!TestCreationForTmp(fileName)) return false;
                if (File.Exists(fileName)) { File.Delete(fileName); }
                File.WriteAllBytes(fileName, data);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Attempts to create a small text file in target folder to confirm write access.
        /// </summary>
        private static bool TestCreationForTmp(string fileName)
        {
            var tmpfile = Path.ChangeExtension(fileName, "txt");
            try
            {
                if (File.Exists(tmpfile)) { File.Delete(tmpfile); }
                File.WriteAllText(tmpfile, "testfile");
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (File.Exists(tmpfile)) { File.Delete(tmpfile); }
            }
        }

        private sealed class DownloadJson
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;

            public bool IsValid
            {
                get
                {
                    if (string.IsNullOrWhiteSpace(Id)) return false;
                    if (string.IsNullOrWhiteSpace(Name)) return false;
                    var directory = Path.GetDirectoryName(Name);
                    if (directory == null) return false;
                    if (!Directory.Exists(directory)) return false;
                    return true;
                }
            }
            public string CalculateFileName()
            {
                const string tmp_name = "record-download-{0}.xlsx";
                if (!IsValid) return tmp_name;
                var shortName = string.Format(tmp_name, DateTime.Now.ToString("yyyyMMdd"));
                var adjustedName = Path.Combine(Name, shortName);
                shortName = Path.GetFileNameWithoutExtension(shortName);
                var indx = 1;
                while (File.Exists(adjustedName))
                {
                    var tmp = $"{shortName}-{indx:D4}.xlsx";
                    adjustedName = Path.Combine(Name, tmp);
                    indx++;
                }
                return adjustedName;
            }
            public static DownloadJson? FromJson(string json)
            {
                try
                {
                    return JsonConvert.DeserializeObject<DownloadJson>(json);
                }
                catch
                {
                    return null;
                }
            }
        }


        private sealed class DownloadJsResponse
        {
            public string? ExternalId { get; set; }
            public string? Description { get; set; }
            public string? Content { get; set; }
            public string? Error { get; set; }
            public string? CreateDate { get; set; }
        }
    }
}