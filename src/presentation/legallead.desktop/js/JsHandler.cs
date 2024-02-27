using CefSharp;
using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            if (ProfileForms.Exists(f => f.Equals(formName, comparison)))
            {
                var handler = new JsProfileChange(web);
                handler.Submit(formName, json);
            }
            if (PermissionForms.Exists(f => f.Equals(formName, comparison)))
            {
                var handler = new JsPermissionChange(web);
                handler.Submit(formName, json);
            }
            if (SearchForms.Exists(f => f.Equals(formName, comparison)))
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
    }
}