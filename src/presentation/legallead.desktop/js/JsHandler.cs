using CefSharp.Wpf;
using legallead.desktop.entities;
using legallead.desktop.interfaces;
using legallead.desktop.utilities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public virtual void Submit(string formName, string json)
        {
            if (ProfileForms.Exists(f => f.Equals(formName, StringComparison.OrdinalIgnoreCase)))
            {
                var handler = new JsProfileChange(web);
                handler.Submit(formName, json);
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
    }
}